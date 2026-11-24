#define _WINSOCK_DEPRECATED_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS

#include <iostream>
#include <winsock2.h>
#include <ctime>
#include <string>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

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
    cout << "Сервер времени/даты запущен и ждет клиента..." << endl;

    sockaddr_in clientAddr;
    int clientSize = sizeof(clientAddr);
    SOCKET clientSocket = accept(serverSocket, (sockaddr*)&clientAddr, &clientSize);

    char request[256] = { 0 };
    recv(clientSocket, request, sizeof(request) - 1, 0);

    time_t now = time(0);
    tm* ltm = localtime(&now);
    char response[256] = { 0 };

    if (string(request) == "1") {
        strftime(response, sizeof(response), "%H:%M:%S", ltm);
    }
    else if (string(request) == "2") {
        strftime(response, sizeof(response), "%d.%m.%Y", ltm);
    }
    else {
        strcpy(response, "Неверный запрос");
    }

    send(clientSocket, response, strlen(response), 0);
    cout << "Ответ отправлен (" << response << "). Разрыв соединения." << endl;

    closesocket(clientSocket);
    closesocket(serverSocket);
    WSACleanup();

    system("pause");
    return 0;
}
