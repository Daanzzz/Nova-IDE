#include <iostream>
#include "EmailHandler.h"

void EmailHandler::sendMail(std::string email) {
    std::srand(static_cast<unsigned int>(std::time(nullptr)));

    // generate a random number in the range [100000, 999999]
    int randomNumber = std::rand() % 900000 + 100000;

    std::string command = "python email_python.py --recipient " + email + " --random_number " + std::to_string(randomNumber);

    // execute the command using the system function
    int result = system(command.c_str());

    // check if the command was executed successfully
    if (!result) {
        this->resetCodes[email] = std::to_string(randomNumber);
    }
    else {
        std::cout << "Failed sending email to " << email << std::endl;
    }
}

bool EmailHandler::isCodeCorrect(std::string email, std::string pass) {
    return this->resetCodes[email] == pass;
}
