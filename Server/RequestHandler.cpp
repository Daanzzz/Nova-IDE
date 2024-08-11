#include "RequestHandler.h"
#include "AccountHandler.h"
#include "Server.h"

#define LOGIN_IDENTIFIER 'L'
#define SIGNUP_IDENTIFIER 'S'
#define SESSION_IDENTIFIER 'J'

RequestHandler::RequestHandler() {
}

RequestHandler::~RequestHandler() {
}

bool RequestHandler::handleRequest(std::string req, struct sockaddr_in clientAddr) {
    bool flag = false;
    std::vector<std::string> res;
    
    res = AccountHandler::getInstance()->analyzeUser(req, clientAddr);


    return flag;
}
