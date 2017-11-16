// event_cmd.cpp : Defines the entry point for the application.
//

#include "StdAfx.h"
#include <string>
#include <afx.h>
#include <iostream>
#include <atlstr.h>
#include <windows.h> 
#include <stdio.h>
#include <conio.h>
#include <tchar.h>


#define MAX_LOADSTRING 100
#define BUFSIZE 128

#ifndef UNICODE  
typedef std::string String;
#else
typedef std::wstring String;
#endif

HANDLE hPipe;
UINT tokenSize;
LPTSTR lpvMessage = TEXT("Default message from client.");
TCHAR  chBuf[BUFSIZE];
BOOL   fSuccess = FALSE;
DWORD  cbRead, cbToWrite, cbToRead, cbWritten, dwMode;

bool  ConnectToPipeServer(String serverName);
bool SendMessageToPipeServer(String msgToSend);


CString GetModPath()
{
	CString path;

	TCHAR szFilePath[_MAX_PATH+1];
	::GetModuleFileName(NULL, szFilePath, _MAX_PATH);
	szFilePath[_MAX_PATH] = 0;

	TCHAR *bsl = _tcsrchr(szFilePath, TCHAR('\\'));
	if(bsl)
	{
		bsl[1] = 0;
		path = CString(szFilePath);
	}

	return path;
}

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR     lpCmdLine,
                     _In_ int       nCmdShow)
{

#ifdef DEBUG
	AllocConsole();
	freopen("CONIN$", "r", stdin);
	freopen("CONOUT$", "w", stdout);
	freopen("CONOUT$", "w", stderr);
#endif

	String cmdLineArg(lpCmdLine);

#ifdef DEBUG
	//wcout << cmdLineArg << '\n';
	//wcin >> cmdLineArg;
	//wcout << cmdLineArg << '\n';
	//wcout << cmdLineArg.length();
#endif

	CString param[100];
	
	int i = cmdLineArg.find(_T(";"));
	int i_prev = -1;
	int j = 0;
	while(i>=0)
	{
		param[j++] = cmdLineArg.substr(i_prev+1, i-i_prev-1).c_str();
		i_prev = i;
		i = cmdLineArg.find(_T(";"), i+1);
	}
	int version = _ttoi(param[0]);
	int eventId = _ttoi(param[1]);
	CString date = param[2];
	CString wnAddr = param[3];
	CString serNumb = param[4];
	CString param1 = param[5];
	CString param2 = param[6];
	CString param3 = param[7];
	CString saName = param[8];
	CString lockName = param[9];
	CString transName = param[10];

	CString outFile = GetModPath() + _T("event_cmd_exe.txt");
	try
	{
		if (ConnectToPipeServer(L"EA2pipe"))		// only create a logfile if PipeServer is not listening 
		{
			CStdioFile ioFile(outFile, CFile::modeCreate | CFile::modeNoTruncate | CFile::modeWrite);
			ioFile.SeekToEnd();
			ioFile.WriteString(CString("ERROR: PIPE SERVER TIMEOUT") + _T("\n"));
			ioFile.WriteString(CString("event_cmd.exe ") + cmdLineArg.c_str() + _T("\n"));

			CString msg;
			if (version != 1)
			{
				msg.Format(_T("Wrong version of event message ver=%d\n"), version);
				ioFile.WriteString(msg);
				return -2;
			}
			if (eventId == 1)
			{
				ioFile.WriteString(_T("LockNode inputs have been changed\n"));
				if (param1 == _T("1"))
					ioFile.WriteString(_T("Input1 changes from 0 to 1\n"));
				if (param1 == _T("2"))
					ioFile.WriteString(_T("Input1 changes from 1 to 0\n"));
				if (param2 == _T("1"))
					ioFile.WriteString(_T("Input2 changes from 0 to 1\n"));
				if (param2 == _T("2"))
					ioFile.WriteString(_T("Input2 changes from 1 to 0\n"));
				if (param3 == _T("1"))
					ioFile.WriteString(_T("Input3 changes from 0 to 1\n"));
				if (param3 == _T("2"))
					ioFile.WriteString(_T("Input3 changes from 1 to 0\n"));
			}
			else if (eventId == 2)
			{
				msg.Format(_T("The door %s has been opened\n"), lockName);
				ioFile.WriteString(msg);
			}
			else if (eventId == 3)
			{
				msg.Format(_T("The door %s stays too long open\n"), lockName);
				ioFile.WriteString(msg);
			}
			else if (eventId == 4)
			{
				msg.Format(_T("The door %s has been closed\n"), lockName);
				ioFile.WriteString(msg);
			}
			else if (eventId == 5)
			{
				msg.Format(_T("The door %s has been locked\n"), lockName);
				ioFile.WriteString(msg);
				msg.Format(_T("Bolt position=%s\n"), param1);
				ioFile.WriteString(msg);
			}
			else if (eventId == 6)
			{
				msg.Format(_T("The door %s has been secured\n"), lockName);
				ioFile.WriteString(msg);
				msg.Format(_T("Bolt position=%s\n"), param1);
				ioFile.WriteString(msg);
			}
			else if (eventId == 7)
			{
				msg.Format(_T("The door/lock %s has been manipulated, code=%s\n"), lockName, param1);
				ioFile.WriteString(msg);
			}
			else if (eventId == 8)
			{
				msg.Format(_T("Sensor error %s has been detected: door %s\n"), param2, lockName);
				ioFile.WriteString(msg);
			}
			else if (eventId == 9)
			{
				msg.Format(_T("Unlocking %s by transponder %s\n"), lockName, transName);
				ioFile.WriteString(msg);
				if (param3 == _T("1"))
				{
					msg.Format(_T("Unlocking was rejected\n"));
					ioFile.WriteString(msg);
				}
			}
			else if (eventId == 10)
			{
				msg.Format(_T("The door %s has been closed after too long opened\n"), lockName);
				ioFile.WriteString(msg);
			}
			else if (eventId == 12)
			{
				msg.Format(_T("Unlocking %s\n"), lockName);
				ioFile.WriteString(msg);
				if (param1 == _T("1"))
				{
					msg.Format(_T("Handle was pressed\n"));
					ioFile.WriteString(msg);
				}
				if (param1 == _T("0"))
				{
					msg.Format(_T("Handle was released\n"));
					ioFile.WriteString(msg);
				}
				if (param2 == _T("1"))
				{
					msg.Format(_T("with transponder\n"));
					ioFile.WriteString(msg);
				}
				if (param2 == _T("0"))
				{
					msg.Format(_T("without transponder\n"));
					ioFile.WriteString(msg);
				}
			}
			else
			{
				msg.Format(_T("Unhandled event id %d\n"), eventId);
				ioFile.WriteString(msg);
			}
		}
		else
		{			
			SendMessageToPipeServer(cmdLineArg);
		}

	}
	catch(...)
	{
		return -1;
	}
	return 0;
}

bool ConnectToPipeServer(String serverName)
{
	try {
		String buildlpszPipeName = L"\\\\.\\pipe\\";
		buildlpszPipeName.append(serverName);

		LPCWSTR lpszPipename = buildlpszPipeName.c_str();

		while (1)	// loop until break
		{
			hPipe = CreateFile(
				lpszPipename,   // pipe name 
				GENERIC_READ |  // read and write access 
				GENERIC_WRITE,
				0,              // no sharing 
				NULL,           // default security attributes
				OPEN_EXISTING,  // opens existing pipe 
				0,              // default attributes 
				NULL);          // no template file 

								// Break if the pipe handle is valid. 

			if (hPipe != INVALID_HANDLE_VALUE)
				return false;

			// Exit if an error other than ERROR_PIPE_BUSY occurs. 

			if (GetLastError() != ERROR_PIPE_BUSY)
			{
				//_tprintf(TEXT("Could not open pipe. GLE=%d\n"), GetLastError());
				return true;
			}

			// All pipe instances are busy, so wait for 20 seconds. 

			if (!WaitNamedPipe(lpszPipename, 5000))
			{
				//_tprintf("Could not open pipe: 5 second wait timed out.");
				throw("ERROR: Timeout");
			}
		}
	}

	catch (...) {
		return false;
	}
	
	return false;
}

bool SendMessageToPipeServer(String msgToSend)
{
	try {

		LPCWSTR lpszMessage = msgToSend.c_str();

		// The pipe connected; change to byte-read mode. and get 2 byte token size

		dwMode = PIPE_READMODE_BYTE;
		fSuccess = SetNamedPipeHandleState(
			hPipe,    // pipe handle 
			&dwMode,  // new pipe mode 
			NULL,     // don't set maximum bytes 
			NULL);    // don't set maximum time 

		if (!fSuccess)
		{
			//_tprintf(TEXT("SetNamedPipeHandleState failed. GLE=%d\n"), GetLastError());
			return true;
		}

		do
		{
			// Read from the pipe. 

			byte temp[2] = { 0,0 };

			for (int i = 0; i < 2; i++)
			{
				fSuccess = ReadFile(
					hPipe,    // pipe handle 
					temp,    // buffer to receive reply 
					BUFSIZE,  // size of buffer 
					&cbRead,  // number of bytes read 
					NULL);    // not overlapped 

				chBuf[i] = temp[0];
			}

			if (!fSuccess && GetLastError() != ERROR_MORE_DATA)
				break;

			//_tprintf(TEXT("\"%s\"\n"), chBuf);

			tokenSize = int((unsigned char)(chBuf[0]) << 8 |
				(unsigned char)(chBuf[1]));

		} while (!fSuccess);  // repeat loop if ERROR_MORE_DATA 

							  // The pipe connected; change to message-read mode. 

		dwMode = PIPE_READMODE_MESSAGE;
		fSuccess = SetNamedPipeHandleState(
			hPipe,    // pipe handle 
			&dwMode,  // new pipe mode 
			NULL,     // don't set maximum bytes 
			NULL);    // don't set maximum time 

		if (!fSuccess)
		{
			printf("SetNamedPipeHandleState failed. GLE=%d\n", GetLastError());
			return true;
		}

		do
		{
			// Read from the pipe.

			fSuccess = ReadFile(
				hPipe,    // pipe handle 
				chBuf,    // buffer to receive reply 
				tokenSize,  // size of buffer 
				&cbRead,  // number of bytes read 
				NULL);    // not overlapped 

			TCHAR token[] = _T("6jEBb4uZ2yLmx39lDESGrhucp6WTUVJKbVdiqySOr4tanvDgGHrBM8OFEFKjDkHmLgTDqYyl0R57RIX0Y61HpjtTJhalzubI4UZhF07HydUckfxGrN5HOk2oLeH62SQj");

			if (!fSuccess && GetLastError() != ERROR_MORE_DATA)
				break;

			if (_tcscmp(chBuf, token))	// Send the message to the pipe server. 
			{
				cbToWrite = ((lstrlen(lpszMessage)) * sizeof(TCHAR));
				//_tprintf(TEXT("Sending %d byte message: \"%s\"\n"), cbToWrite, lpvMessage);

				chBuf[0] = cbToWrite & 255;
				chBuf[1] = cbToWrite / 256;


				fSuccess = WriteFile(
					hPipe,                  // pipe handle 
					chBuf,					// message 
					4,						// message length 
					&cbWritten,             // bytes written 
					NULL);                  // not overlapped 


				fSuccess = WriteFile(
					hPipe,                  // pipe handle 
					lpszMessage,             // message 
					cbToWrite,              // message length 
					&cbWritten,             // bytes written 
					NULL);                  // not overlapped 

				if (!fSuccess)
				{
					//_tprintf(TEXT("WriteFile to pipe failed. GLE=%d\n"), GetLastError());
					return true;
				}

#ifdef DEBUG
				//wcout << "\nMessage sent to server, receiving reply as follows:\n";
#endif


				if (!fSuccess)
				{
					//_tprintf(TEXT("ReadFile from pipe failed. GLE=%d\n"), GetLastError());
					return true;
				}

				//printf("\n<End of message, press ENTER to terminate connection and exit>");
				//_getch();

				//CloseHandle(hPipe);
			}
			//_tprintf(TEXT("yep got you"));

			//_tprintf(TEXT("\"%s\"\n"), chBuf);
		} while (!fSuccess);  // repeat loop if ERROR_MORE_DATA 
	}
	catch (...)
	{
	}
	return 0;
}

