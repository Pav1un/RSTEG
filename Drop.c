#include <winsock2.h>
#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

#include "windivert.h"

#define MAXBUF  0xFFFF   
#define BUFSTEG 13

typedef struct
{
    WINDIVERT_IPHDR ip;
    WINDIVERT_TCPHDR tcp;
} TCPPACKET, *PTCPPACKET;



int main (int argc, char** argv){
    HANDLE handle;         
    WINDIVERT_ADDRESS addr; 
    char packet[MAXBUF];  
    UINT packetLen;   
    int countCapture = srand(time(NULL)) % 4;
    
   
    if (argv[1] == NULL){
        fprintf(stderr, "get parametrs..\n"); 
        exit(2);
    }

    handle = WinDivertOpen("tcp.SrcPort == 1786", 0, 0, 0);   

    if (handle == INVALID_HANDLE_VALUE)
    {
        printf("%s\n",GetLastError());
        exit(1);
    }

    while (countCapture != 0)
    {
        if (!WinDivertRecv(handle, packet, sizeof(packet), &addr, &packetLen))
        {
            printf("%s\n",GetLastError());
            continue;
        }
        else {
            WinDivertHelperParsePacket(packet, packetLen, NULL,
                        NULL, NULL, NULL, &tcp_header,
                        NULL, NULL, NULL);
            printf("SrcPort = %u\t DstPort = %u\t AckNum = %u\t \n ================================================\n ",
                 ntohs(tcp_header->SrcPort), ntohs(tcp_header->DstPort), ntohl(tcp_header->AckNum));
            printf("%s\n", );
            countCapture -= 1;
        }
            
    }

}



