#define _WINSOCK_DEPRECATED_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS

#include <iostream>
#include <winsock2.h>
#include <ctime>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

void printTime() {
    time_t now = time(0);
    tm* ltm = localtime(&now);
    printf("%02d:%02d", ltm->tm_hour, ltm->tm_min);
}

int main() {
    setlocale(LC_ALL, "Russian");

    WSADATA wsaData;
    WSAStartup(MAKEWORD(2, 2), &wsaData);

    SOCKET clientSocket = socket(AF_INET, SOCK_STREAM, 0);

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");
    serverAddr.sin_port = htons(12345);

    connect(clientSocket, (sockaddr*)&serverAddr, sizeof(serverAddr));

    const char* msg = "Привет, сервер!";
    send(clientSocket, msg, strlen(msg), 0);

    char buffer[256] = { 0 };
    recv(clientSocket, buffer, sizeof(buffer) - 1, 0);

    cout << "В ";
    printTime();
    cout << " от 127.0.0.1 получена строка: " << buffer << endl;

    closesocket(clientSocket);
    WSACleanup();

    system("pause");
    return 0;
}
