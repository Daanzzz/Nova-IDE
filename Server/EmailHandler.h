#pragma once

#include <string>
#include <map>
#include <cstdlib>
#include <ctime>
#include <random>

class EmailHandler {
public:
    static EmailHandler& getInstance() {
        static EmailHandler instance;
        return instance;
    }

    EmailHandler(const EmailHandler&) = delete;
    EmailHandler& operator=(const EmailHandler&) = delete;

    // mail operations
    void sendMail(std::string email);
    bool isCodeCorrect(std::string email, std::string pass);

private:
    EmailHandler() {}
    ~EmailHandler() {}

    std::map<std::string, std::string> resetCodes; // holds password reset codes by name
};
