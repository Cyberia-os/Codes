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

    SOCKET serverSocket = socket(AF_INET, SOCK_STREAM, 0);

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(12345);
    bind(serverSocket, (sockaddr*)&serverAddr, sizeof(serverAddr));

    listen(serverSocket, 1);
    cout << "Сервер запущен и ждет клиента..." << endl;

    sockaddr_in clientAddr;
    int clientSize = sizeof(clientAddr);
    SOCKET clientSocket = accept(serverSocket, (sockaddr*)&clientAddr, &clientSize);

    char buffer[256] = { 0 };
    recv(clientSocket, buffer, sizeof(buffer) - 1, 0);

    cout << "В ";
    printTime();
    cout << " от " << inet_ntoa(clientAddr.sin_addr) << " получена строка: " << buffer << endl;

    const char* reply = "Привет, клиент!";
    send(clientSocket, reply, strlen(reply), 0);

    closesocket(clientSocket);
    closesocket(serverSocket);
    WSACleanup();

    system("pause");
    return 0;
}
