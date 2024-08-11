#include "Server.h"
#include <exception>
#include <iostream>
#include <string>
#include <thread>
#include <ws2tcpip.h>
#include "SocketHandler.h"
#include "RequestHandler.h"

#define SERVER_IP "127.0.0.1"

// each request will have its own code
#define LOGIN_CODE '1'
#define SESSION_CODE '8'

#pragma warning(disable:4996) 
#pragma comment(lib, "ws2_32.lib") // Link with the Winsock library
#define _WINSOCK_DEPREACTED_NO_WARNINGS

Server::Server() {
    this->infoExchangeFlag = true;

    // This server uses UDP, so we will use SOCK_DGRAM & IPPROTO_UDP
    _serverSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

    if (_serverSocket == INVALID_SOCKET)
        throw std::exception(__FUNCTION__ " - socket");
}

Server& Server::getInstance() {
    static Server instance;
    return instance;
}

Server::~Server() {
    try {
        // The only use of the destructor should be for freeing resources that were allocated in the constructor
        closesocket(_serverSocket);
    }
    catch (...) {}
}

std::string ipChecker(char* ip) {
    if (ip[0] == '1' && ip[1] == '9' && ip[2] == '2') {
        return SERVER_IP;
    }

    return std::string(ip);
}

void Server::serve(int port) {
    struct sockaddr_in sa = { 0 };
    bool flag = true;

    sa.sin_port = htons(port); // Port that the server will listen to
    sa.sin_family = AF_INET;   // Must be AF_INET
    sa.sin_addr.s_addr = INADDR_ANY;    // When there are multiple IPs for the machine, always use "INADDR_ANY"

    // Bind the socket to the configuration (port and etc.)
    if (bind(_serverSocket, (struct sockaddr*)&sa, sizeof(sa)) == SOCKET_ERROR)
        throw std::exception(__FUNCTION__ " - bind");

    std::cout << "Listening on port " << port << std::endl;

    std::thread msgReceiver(&Server::acceptClient, this);
    msgReceiver.detach();
}

void Server::sendResponse(std::string res, std::string username) {
    // Extract IP address and port from clientAddr
    char clientIP[INET_ADDRSTRLEN];
    inet_ntop(AF_INET, &(this->clientAddresses[username].sin_addr), clientIP, INET_ADDRSTRLEN);
    int clientPort = ntohs(this->clientAddresses[username].sin_port);

    sendto(_serverSocket, res.c_str(), size(res), 0, (struct sockaddr*)&this->clientMap[username], sizeof(this->clientMap[username]));
}

void Server::sendResponseByAddr(std::string res, struct sockaddr_in clientAddr) {
    // Extract IP address and port from clientAddr
    char clientIP[INET_ADDRSTRLEN];
    inet_ntop(AF_INET, &(clientAddr.sin_addr), clientIP, INET_ADDRSTRLEN);
    int clientPort = ntohs(clientAddr.sin_port);

    sendto(_serverSocket, res.c_str(), size(res), 0, (struct sockaddr*)&clientAddr, sizeof(clientAddr));
}

int Server::exchangeInfo(std::string client1, std::string client2) {
    bool sentFlag = false;
    
    while (!sentFlag) {
        if (this->sessionInvites[client1] == client2 && this->sessionInvites[client2] == client1) {
            sentFlag = true;

            // Extract IP address and port from clientAddr
            // First client
            char clientIP1[INET_ADDRSTRLEN];
            inet_ntop(AF_INET, &(this->clientAddresses[client1].sin_addr), clientIP1, INET_ADDRSTRLEN);
            int clientPort1 = ntohs(this->clientAddresses[client1].sin_port);

            // Second client
            char clientIP2[INET_ADDRSTRLEN];
            inet_ntop(AF_INET, &(this->clientAddresses[client2].sin_addr), clientIP2, INET_ADDRSTRLEN);
            int clientPort2 = ntohs(this->clientAddresses[client2].sin_port);

            std::string cIP1 = ipChecker(clientIP1);
            std::string cIP2 = ipChecker(clientIP2);

            // Create strings with IP:Port information
            std::string info1 = cIP1 + ":" + std::to_string(clientPort1) + ":C";
            std::string info2 = cIP2 + ":" + std::to_string(clientPort2) + ":H";

            std::cout << "\n";
            std::cout << info1 << std::endl;
            std::cout << info2 << std::endl;

            sendto(_serverSocket, info2.c_str(), size(info2), 0, (struct sockaddr*)&this->clientMap[client1], sizeof(this->clientMap[client1]));
            sendto(_serverSocket, info1.c_str(), size(info1), 0, (struct sockaddr*)&this->clientMap[client2], sizeof(this->clientMap[client2]));
        }
    }

    return 1;
}

void Server::enterSession(std::string current, std::string wanted) {
    this->sessionInvites[current] = wanted;
    
    if (this->infoExchangeFlag) { // prevents sending two exchange msgs
        this->infoExchangeFlag = false;
        exchangeInfo(current, wanted);
    }
}

void Server::addClientAddress(std::string name, sockaddr_in clientAddr) {
    this->clientAddresses[name] = clientAddr;
    this->clientMap[name] = clientAddr;
}

SOCKET Server::getServerSocket() {
    return this->_serverSocket;
}

void Server::acceptClient() {
    while (true) {
        //const std::string acceptedResponse = "OK";

        std::cout << "W8ing 4 msgS" << std::endl;

        struct sockaddr_in clientAddr;
        int clientAddrSize = sizeof(clientAddr);

        char buffer[256]; // You can use this buffer to receive data from clients
        memset(buffer, 0, sizeof(buffer)); // cleanin' da buffa

        // receiving request
        int bytesReceived = recvfrom(_serverSocket, buffer, sizeof(buffer), 0, (struct sockaddr*)&clientAddr, &clientAddrSize);
        if (bytesReceived == SOCKET_ERROR)
            throw std::exception(__FUNCTION__);
        std::string request = std::string(buffer);

        RequestHandler* hReq = new RequestHandler();
        std::thread accountThread(&RequestHandler::handleRequest, hReq, request, clientAddr);
        accountThread.detach();
    }
}
