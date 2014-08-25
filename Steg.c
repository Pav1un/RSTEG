 #include <winsock2.h>
#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "windivert.h"

#define MAXBUF  0xFFFF   
#define BUFSTEG 13

typedef struct
{
    WINDIVERT_IPHDR ip;
    WINDIVERT_TCPHDR tcp;
} TCPPACKET, *PTCPPACKET;

typedef struct
{
    WINDIVERT_IPV6HDR ipv6;
    WINDIVERT_TCPHDR tcp;
} TCPV6PACKET, *PTCPV6PACKET;
 
typedef struct
{
    WINDIVERT_IPHDR ip;
    WINDIVERT_ICMPHDR icmp;
    UINT8 data[];
} ICMPPACKET, *PICMPPACKET;

typedef struct
{
    WINDIVERT_IPV6HDR ipv6;
    WINDIVERT_ICMPV6HDR icmpv6;
    UINT8 data[];
} ICMPV6PACKET, *PICMPV6PACKET;

  
void printArray (char* m);

int main (int argc, char** argv){
    HANDLE handle;          
    WINDIVERT_ADDRESS addr; 
    char packet[MAXBUF];  
    char backPacket[MAXBUF];  
    UINT packetLen;   
    PWINDIVERT_TCPHDR tcp_header;
    PWINDIVERT_TCPHDR mytcp_header;
    PWINDIVERT_IPHDR ip_header; 
    PVOID ppData;
    UINT pDataLenn;
    int count = 1;
    //char stegWord[BUFSTEG];
    char *pStegWord;
    int lengthKeyWord;

int i;
  
    

    switch(argc)
    {
        case 1:
            printf("%s\n", "input tcp options..");
            exit(0);
        case 2: 
            pStegWord = argv[1];
            lengthStegWord = strlen(argv[1]);
            break;
        default:
            printf("%s\n", "1: nameProgramm; 2: optionsTCP");
            exit(EXIT_FAILURE);
    }

    handle = WinDivertOpen("tcp.DstPort == 1786", 0, 0, 0);   // Open filter 

    if (handle == INVALID_HANDLE_VALUE)
    {
        printf("%s\n", GetLastError());
        exit(1);
    }

   
    while (count != 0)
    {
        if (!WinDivertRecv(handle, packet, sizeof(packet), &addr, &packetLen))
        {
            printf("%s\n", GetLastError());
            continue;
        }
        else {
            WinDivertHelperParsePacket(packet, packetLen, &ip_header,
                        NULL, NULL, NULL, &tcp_header,
                        NULL, &ppData, &pDataLenn);

            printf("SrcPort = %u\t DstPort = %u\t AckNum = %u\t \n ================================================\n ",
                 ntohs(tcp_header->SrcPort), ntohs(tcp_header->DstPort), ntohl(tcp_header->AckNum));
                

            memcpy(backPacket, packet, sizeof(packet));
            
           // printf("%s\n", ppData);
           
            for (i = 0; i<=lengthStegWord; i++){
                if(backPacket + 41 + i  != NULL)
                    backPacket[41 + i] = (char)*(pStegWord + i);
            }
            /*
            //printf("%s\n",backPacket[] );
               if( (backPacket + 42 != NULL) && (backPacket + 41 != NULL) ){
                     backPacket[41] = 'n';
                     backPacket[42] = 'o';
                }*/
                //printf("%s\n", backPacket[41]);
                if (!WinDivertSend(handle, backPacket, packetLen, &addr, NULL))
                {
                    printf("%s\n", GetLastError());
                    continue;
                }
                else
                {
                    count = count - 1;
                }
         
        }   
    }
}


