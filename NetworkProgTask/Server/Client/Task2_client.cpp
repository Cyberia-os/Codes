#define _WINSOCK_DEPRECATED_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS

#include <iostream>
#include <winsock2.h>
#include <string>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

int main() {
    setlocale(LC_ALL, "Russian");

    WSADATA wsaData;
    WSAStartup(MAKEWORD(2, 2), &wsaData);

    SOCKET clientSocket = socket(AF_INET, SOCK_STREAM, 0);

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");
    serverAddr.sin_port = htons(12345);

    string choice;
    cout << "Что запросить у сервера? (1 - время, 2 - дата): ";
    cin >> choice;

    connect(clientSocket, (sockaddr*)&serverAddr, sizeof(serverAddr));
    send(clientSocket, choice.c_str(), choice.length(), 0);

    char buffer[256] = { 0 };
    recv(clientSocket, buffer, sizeof(buffer) - 1, 0);
    cout << "Ответ сервера: " << buffer << endl;

    closesocket(clientSocket);
    WSACleanup();

    system("pause");
    return 0;
}
