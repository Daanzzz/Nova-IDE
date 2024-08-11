#pragma once

#include <string>
#include <vector>

class RequestHandler {
public:
    RequestHandler();
    ~RequestHandler();

    bool handleRequest(std::string req, struct sockaddr_in clientAddr);
};
