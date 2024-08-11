#pragma once

#include <WinSock2.h>
#include <Windows.h>
#include <vector>
#include <map>
#include <string>

class Server {
public:
	static Server& getInstance();
	~Server();

	void serve(int port);
	void sendResponse(std::string res, std::string username);
	int exchangeInfo(std::string client1, std::string client2);
	void enterSession(std::string current, std::string wanted);
	void sendResponseByAddr(std::string res, struct sockaddr_in clientAddr);
	void addClientAddress(std::string name, struct sockaddr_in clientAddr);
	SOCKET getServerSocket();

private:
	// class related
	Server();

	// client info
	std::map<std::string ,struct sockaddr_in> clientAddresses;
	std::map<std::string, sockaddr_in> clientMap;
	std::map<std::string, std::string> sessionInvites; // index 0 = user1 , index 1 = user2

	void acceptClient();
	void clientHandler(SOCKET clientSocket);

	bool infoExchangeFlag;
	SOCKET _serverSocket;
};
