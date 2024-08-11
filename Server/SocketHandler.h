#pragma once
#include <WinSock2.h>
#include <string>

class SocketHandler {
public:
	SocketHandler(std::string uname, struct sockaddr_in addr, int addrSize);
	~SocketHandler();

	void handleSocket(std::string uname, std::string otherUser);

private:
	struct sockaddr_in clientAddr;
	int clientAddrSize;
	std::string username;
};