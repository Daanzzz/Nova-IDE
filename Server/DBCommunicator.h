#pragma once
#include "sqlite3.h"
#include <string>
using std::string;

static class DBCommunicator
{
public:
	static bool open();
	static bool close();
	static int doesUserExist(const string username);
	static int doesEmailExist(const string email);
	static int doesPasswordMatch(const string username, const string password);
	static int addNewUser(const string& username, const string& password, const string& mail);
	static int changePassword(const string username, const string password);
	static int doesEmailMatch(const string username, const string email);


private:
	static sqlite3* db;

};

