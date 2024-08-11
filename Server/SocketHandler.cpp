#include "SocketHandler.h"
#include "Server.h"

SocketHandler::SocketHandler(std::string uname, struct sockaddr_in addr, int addrSize) {
	this->clientAddr = addr;
	this->clientAddrSize = addrSize;
	this->username = uname;
}

SocketHandler::~SocketHandler() {
}

void SocketHandler::handleSocket(std::string uname, std::string otherUser) {

	// exchanging info
	Server::getInstance().enterSession(uname, otherUser);
	
}
