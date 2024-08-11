#pragma once

#include <string>
#include <vector>
#include <random>
#include <map>

class AccountHandler {
public:
    static AccountHandler* getInstance() {
        if (!instance) {
            instance = new AccountHandler();
        }
        return instance;
    }

    AccountHandler(const AccountHandler&) = delete;
    AccountHandler& operator=(const AccountHandler&) = delete;

    // request handlers
    std::vector<std::string> analyzeUser(std::string userInfo, struct sockaddr_in clientAddr);
    std::vector<std::string> handleLogin(std::string userInfo);
    std::vector<std::string> handleSignup(std::string userInfo);
    std::vector<std::string> handleSessionReq(std::string userInfo);
    std::vector<std::string> handlePassReset(std::string userInfo);
    std::vector<std::string> handleEmailReq(std::string userInfo);
    std::vector<std::string> verifyPasscode(std::string userInfo);

    // utility functions
    std::vector<std::string> getDetails(std::string userInfo);
    std::string generateSessionToken();

    // communication operations
    void sendResponse(std::vector<std::string> res, std::string name);
    void sendResponseByAddr(std::vector<std::string> res, struct sockaddr_in clientAddr);

private:
    AccountHandler() {}

    static AccountHandler* instance;
    std::map<std::string, std::string> usersList; // < token, username >
};
