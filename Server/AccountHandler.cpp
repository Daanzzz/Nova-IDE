#include "AccountHandler.h"
#include "DBCommunicator.h"
#include "Server.h"
#include "EmailHandler.h"

// request identifiers
#define LOGIN_IDENTIFIER 'L'
#define SIGNUP_IDENTIFIER 'S'
#define SESSION_IDENTIFIER 'J'
#define MAIL_CODE_IDENTIFIER 'V'
#define CODE_CHECK_IDENTIFIER 'C'
#define NEW_PASS_IDENTIFIER 'N'
#define SEPERATION_IDENTIFIER ':'

// response codes
#define INCORRECT_PASS 1
#define INVALID_USER 2
#define USER_ALREADY_EXISTS 3
#define EMAIL_ALREADY_EXISTS 4
#define EMAIL_DOESNT_EXISTS 5
#define INFO_CORRECT 9

AccountHandler* AccountHandler::instance = nullptr;

// I:USER:PASS
std::vector<std::string> AccountHandler::analyzeUser(std::string userInfo, struct sockaddr_in clientAddr) {
	std::vector<std::string> result;
	DBCommunicator::open();

	if (userInfo[0] == LOGIN_IDENTIFIER) {
		result = handleLogin(userInfo);

		if (std::stoi(result[0]) == INFO_CORRECT) {
			std::string usersToken = generateSessionToken();
			this->usersList[usersToken] = result[1];

			// removing the username (was temp) and adding the token
			result.pop_back();
			result.push_back(usersToken);

			Server::getInstance().addClientAddress(this->usersList[usersToken], clientAddr);
			sendResponse(result, this->usersList[usersToken]);
		}
		else {
			sendResponseByAddr(result, clientAddr);
		}
	}
	else if (userInfo[0] == SIGNUP_IDENTIFIER) {
		result = handleSignup(userInfo);

		sendResponseByAddr(result, clientAddr);
	}
	else if (userInfo[0] == SESSION_IDENTIFIER) {
		result = handleSessionReq(userInfo);

		Server::getInstance().enterSession(result[0], result[1]);
	}
	else if (userInfo[0] == CODE_CHECK_IDENTIFIER) { // for signup
		result = verifyPasscode(userInfo);

		sendResponseByAddr(result, clientAddr);
	}
	else if (userInfo[0] == MAIL_CODE_IDENTIFIER) { // for pass reset
		result = handleEmailReq(userInfo);

		sendResponseByAddr(result, clientAddr);
	}
	else if (userInfo[0] == NEW_PASS_IDENTIFIER) {
		result = handlePassReset(userInfo);

		sendResponseByAddr(result, clientAddr);
	}

	return result;
}

std::vector<std::string> AccountHandler::handleLogin(std::string userInfo) {
	std::vector<std::string> finalResult;

	std::vector<std::string> details = getDetails(userInfo);
	std::string username = details[0];
	std::string password = details[1];

	if (DBCommunicator::doesUserExist(username)) {
		if (DBCommunicator::doesPasswordMatch(username, password)) {
			finalResult.push_back(std::to_string(INFO_CORRECT)); // user name & pass correct
			finalResult.push_back(username);
		}
		else {
			finalResult.push_back(std::to_string(INCORRECT_PASS)); // incorrect pass
		}
	}
	else {
		finalResult.push_back(std::to_string(INVALID_USER)); // user doesnt exist
	}

	return finalResult;
}

std::vector<std::string> AccountHandler::handleSignup(std::string userInfo) {
	std::vector<std::string> result;

	std::vector<std::string> details = getDetails(userInfo);
	std::string username = details[0];
	std::string password = details[1];
	std::string email = details[2];

	// need to add email check if exists already
	if (!(DBCommunicator::doesUserExist(username))) {
		if (!(DBCommunicator::doesEmailExist(email))) {
			result.push_back(std::to_string(INFO_CORRECT));
			result.push_back(username);

			EmailHandler::getInstance().sendMail(email);
		}
		else {
			result.push_back(std::to_string(EMAIL_ALREADY_EXISTS));
		}
	}
	else {
		result.push_back(std::to_string(USER_ALREADY_EXISTS));
	}

	return result;
}

std::vector<std::string> AccountHandler::handleSessionReq(std::string userInfo) {
	std::vector<std::string> result;

	std::vector<std::string> details = getDetails(userInfo);
	std::string token1 = this->usersList[details[0]]; // first user (requester) is represented by token, so we retrieve the name
	std::string token2 = details[1];

	result.push_back(token1);
	result.push_back(token2);

	return result;
}

std::vector<std::string> AccountHandler::handlePassReset(std::string userInfo) {
	std::vector<std::string> result;

	std::vector<std::string> details = getDetails(userInfo);
	std::string username = details[0];
	std::string newPassword = details[1];
	std::string email = details[2];
	std::string passcode = details[3];
	
	if (EmailHandler::getInstance().isCodeCorrect(email, passcode)) {
		DBCommunicator::changePassword(username, newPassword);
		result.push_back(std::to_string(INFO_CORRECT));
	}
	else {
		result.push_back(std::to_string(INCORRECT_PASS));
	}

	return result;
}

std::vector<std::string> AccountHandler::handleEmailReq(std::string userInfo) {
	std::vector<std::string> result;

	std::vector<std::string> details = getDetails(userInfo);
	std::string username = details[0];
	std::string email = details[1];
	
	if (DBCommunicator::doesEmailExist(email)) {
		if (DBCommunicator::doesEmailMatch(username, email)) {
			EmailHandler::getInstance().sendMail(email);
			result.push_back(std::to_string(INFO_CORRECT));
		}
		else {
			result.push_back(std::to_string(INVALID_USER));
		}
	}
	else {
		result.push_back(std::to_string(EMAIL_DOESNT_EXISTS));
	}

	return result;
}

std::vector<std::string> AccountHandler::verifyPasscode(std::string userInfo) {
	std::vector<std::string> result;

	std::vector<std::string> details = getDetails(userInfo);
	std::string username = details[0];
	std::string password = details[1];
	std::string email = details[2];
	std::string code = details[3];


	if (EmailHandler::getInstance().isCodeCorrect(email, code)) {
		DBCommunicator::addNewUser(username, password, email);
		result.push_back(std::to_string(INFO_CORRECT));
	}
	else {
		result.push_back(std::to_string(INCORRECT_PASS));
	}

	return result;
}

std::vector<std::string> AccountHandler::getDetails(std::string userInfo) {
	std::vector<std::string> details;
	std::string tStr = "";

	for (int i = 2; i < userInfo.size(); i++) {
		if (userInfo[i] == SEPERATION_IDENTIFIER) {
			details.push_back(tStr);
			tStr = "";
		}
		else if (userInfo[i] == '@' || userInfo[i] == '.' || std::isalpha(static_cast<unsigned char>(userInfo[i])) || std::isdigit(static_cast<unsigned char>(userInfo[i]))) {
			tStr += userInfo[i];
		}
	}

	details.push_back(tStr);

	return details;
}

std::string AccountHandler::generateSessionToken() {
	int length = 12;
	const std::string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"; // You can include other characters if needed

	std::random_device rd;
	std::mt19937 gen(rd());
	std::uniform_int_distribution<> dis(0, charset.size() - 1);
	std::string token;

	for (int i = 0; i < length; ++i) {
		token += charset[dis(gen)];
	}
	return token;
}

void AccountHandler::sendResponse(std::vector<std::string> res, std::string name) {
	std::string finalRes = "";

	for (int i = 0; i < res.size(); i++) {
		finalRes = finalRes + res[i] + ':';
	}
	finalRes.pop_back(); // removing extra ':'

	Server::getInstance().sendResponse(finalRes, name);
}

void AccountHandler::sendResponseByAddr(std::vector<std::string> res, struct sockaddr_in clientAddr) {
	std::string finalRes = "";

	for (int i = 0; i < res.size(); i++) {
		finalRes = finalRes + res[i] + ':';
	}
	finalRes.pop_back(); // removing extra ':'

	Server::getInstance().sendResponseByAddr(finalRes, clientAddr);
}
