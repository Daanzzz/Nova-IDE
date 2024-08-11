#include "DBCommunicator.h"
#include <iostream>
#include <io.h>

using std::cout;
using std::endl;
using std::to_string;

sqlite3* DBCommunicator::db = nullptr;
#define DBFILENAME "NovaDB.sqlite"

bool DBCommunicator::open()
{
	string command;
	int res = 0, exists = 0;
	char* errMessage = nullptr;
	string dbFileName = DBFILENAME;
	exists = _access(dbFileName.c_str(), 0);

	res = sqlite3_open(dbFileName.c_str(), &db);
	if (res != SQLITE_OK) {
		db = nullptr;
		return false;
	}

	//creating the Users table
	command = "CREATE TABLE IF NOT EXISTS Users(id integer primary key autoincrement not null, Username text not null, Password text not null, Mail text not null); ";
	res = sqlite3_exec(db, command.c_str(), nullptr, nullptr, &errMessage);
	if (res != SQLITE_OK) {
		return false;
	}

	return true;
}

bool DBCommunicator::close()
{
	int res = 0;
	res = sqlite3_close(db);
	if (res != SQLITE_OK) {
		db = nullptr;
		return false;
	}

	db = nullptr;
	return true;

}

int callbackItemExists(void* data, int argc, char** argv, char** azColName)
{
	bool* exists = static_cast<bool*>(data);

	if (argc) {
		*exists = true;
	}
	else {
		*exists = false;
	}

	return 0;
}


int DBCommunicator::doesUserExist(const string username)
{
	string command;
	int res = 0;
	char* errMessage = nullptr;
	bool exists = false;

	command = "select * from Users where Username = '" + username + "'; ";
	res = sqlite3_exec(db, command.c_str(), callbackItemExists, &exists, &errMessage);

	return exists;
}

int DBCommunicator::doesEmailExist(const string email)
{
	string command;
	int res = 0;
	char* errMessage = nullptr;
	bool exists = false;

	command = "select * from Users where Mail = '" + email + "'; ";
	res = sqlite3_exec(db, command.c_str(), callbackItemExists, &exists, &errMessage);
	if (res != SQLITE_OK) {
		cout << errMessage << endl;
	}
	return exists;
}


int callbackGetValueByUsername(void* data, int argc, char** argv, char** azColName)
{
	string* DBPassword = static_cast<string*>(data);
	//getting the first result the command in the next function gives
	*DBPassword = argv[0];

	return 0;

}


int DBCommunicator::doesPasswordMatch(const string username, const string password)
{
	string command;
	int res = 0;
	char* errMessage = nullptr;
	string DBPassword;

	command = "select Password from Users where Username = '" + username + "'; ";
	res = sqlite3_exec(db, command.c_str(), callbackGetValueByUsername, &DBPassword, &errMessage);

	if (DBPassword == password) {
		return true;
	}
	else {
		return false;
	}

}

int DBCommunicator::addNewUser(const string& username, const string& password, const string& mail)
{
	string command;
	int res = 0;
	char* errMessage = nullptr;

	command = "insert into Users (Username, Password, Mail) values ('" + username + "', '" + password + "', '" + mail + "'); ";
	res = sqlite3_exec(db, command.c_str(), nullptr, nullptr, &errMessage);
	if (res != SQLITE_OK) {
		return false;
	}

	return true;
}

int DBCommunicator::changePassword(const string username, const string password)
{
	string command;
	int res = 0;
	char* errMessage = nullptr;

	command = "update Users set password = '" + password + "' where username = '" + username + "';";
	res = sqlite3_exec(db, command.c_str(), nullptr, nullptr, &errMessage);
	if (res != SQLITE_OK) {
		cout << errMessage << endl;
		return false;
	}

	return true;
}

int DBCommunicator::doesEmailMatch(const string username, const string email)
{
	string command;
	int res = 0;
	char* errMessage = nullptr;
	string DBValue;

	command = "select Mail from Users where Username = '" + username + "'; ";
	res = sqlite3_exec(db, command.c_str(), callbackGetValueByUsername, &DBValue, &errMessage);
	if (res != SQLITE_OK) {
		cout << errMessage << endl;
	}

	if (DBValue == email) {
		return true;
	}
	else {
		return false;
	}
}
