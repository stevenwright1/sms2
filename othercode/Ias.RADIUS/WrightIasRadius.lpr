library WrightIasRadius;

{ $mode objfpc }
{$MODE DELPHI}
{$H+}

uses
  Windows,
  JwaWinType,
  JwaWinError,
  JwaNtSecApi,
  JwaAuthif,
  Sysutils;

{$R *.res}

const
  RADIUS_ATTR_NOT_FOUND: DWORD = DWORD(-1);

var PolicyHandle: LSA_HANDLE;

  procedure DLLEntryPoint(dwReason: DWORD);
  begin
    if (dwReason = DLL_PROCESS_ATTACH) then
    begin
      DisableThreadLibraryCalls(hInstance);
    end;
  end;

  procedure Process_Detach_Hook(dllparam: longint);
  begin
    DLLEntryPoint(DLL_PROCESS_DETACH);
  end;

  procedure Thread_Attach_Hook(dllparam: longint);
  begin
    DLLEntryPoint(DLL_THREAD_ATTACH);
  end;

  procedure Thread_Detach_Hook(dllparam: longint);
  begin
    DLLEntryPoint(DLL_THREAD_DETACH);
  end;

  function RadiusFindFirstIndex(pAttrs: PRADIUS_ATTRIBUTE_ARRAY;
    dwAttrType: DWORD): DWORD;
  var
    dwIndex, dwSize: DWORD;
    pAttr: PRADIUS_ATTRIBUTE;
  begin
    if (pAttrs = nil) then
    begin
      Result := RADIUS_ATTR_NOT_FOUND;
      Exit;
    end;

    dwSize := pAttrs^.GetSize(pAttrs);

    for dwIndex := 0 to dwSize - 1 do
    begin
      pAttr := pAttrs^.AttributeAt(@pAttrs, dwIndex);
      if (pAttr^.dwAttrType = dwAttrType) then
      begin
        Result := dwIndex;
        Exit;
      end;
    end;

    Result := RADIUS_ATTR_NOT_FOUND;
  end;

  function RadiusFindFirstAttribute(pAttrs: PRADIUS_ATTRIBUTE_ARRAY;
    dwAttrType: DWORD): PRADIUS_ATTRIBUTE;
  var
    dwIndex: DWORD;
  begin
    dwIndex := RadiusFindFirstIndex(pAttrs, dwAttrType);
    if (dwIndex <> RADIUS_ATTR_NOT_FOUND) then
    begin
      Result:=pAttrs^.AttributeAt(@pAttrs, dwIndex);
    end
    else
    begin
      Result:=nil;
    end;
  end;

  function RadiusExtensionInit(): DWORD; stdcall;
  var
    ObjectAttribs: LSA_OBJECT_ATTRIBUTES;
    nts: NTSTATUS;
  begin
       ZeroMemory(@ObjectAttribs, SizeOf(ObjectAttribs));
       nts := LsaOpenPolicy(nil, ObjectAttribs, POLICY_LOOKUP_NAMES, PolicyHandle);
       Result:= LsaNtStatusToWinError(nts);
  end;

  procedure RadiusExtensionTerm(); stdcall;
  begin
    LsaClose(PolicyHandle);
    PolicyHandle:=nil;
  end;

  function RadiusExtensionProcess2(pECB: PRADIUS_EXTENSION_CONTROL_BLOCK): DWORD; stdcall;
  var
    attrArray: PRADIUS_ATTRIBUTE_ARRAY;
    attr: PRADIUS_ATTRIBUTE;
    File1: TextFile;
  begin
    Result:=NO_ERROR;
    try
       AssignFile(File1, 'C:\radius\tmp\fpc.txt');
       Append(File1);
       WriteLn(File1, 'Here 1');
       Flush(File1);
       if pECB^.repPoint <> repAuthentication then
            Exit;

       WriteLn(File1, 'Here 2');
       Flush(File1);

       if ((pECB^.rcRequestType <> rcAccessRequest) and
          (pECB^.rcRequestType <> rcAccessChallenge)) then
            Exit;

       WriteLn(File1, 'Here 3');
       Flush(File1);

       if (pECB^.rcResponseType = rcAccessReject) then
            Exit;

       WriteLn(File1, 'Here 4');
       Flush(File1);

       attrArray := pECB^.GetRequest(pECB);

       WriteLn(File1, 'Here 4,4');
       Flush(File1);

       attr := RadiusFindFirstAttribute(attrArray, ratProvider);
       if (attr = nil) or (attr^.dwValue <> dword(rapWindowsNT)) then
            Exit;

       WriteLn(File1, 'Here 5');
       Flush(File1);


       attr := RadiusFindFirstAttribute(attrArray, ratUserName);
       if (attr = nil) then
            Exit;

       WriteLn(File1, 'Here 6');
       Flush(File1);

       WriteLn(File1, PChar(attr^.lpValue));

       WriteLn(File1, 'Here 7');
       Flush(File1);

       CloseFile(File1);
    except
          on E: EInOutError do
          begin
               Writeln('File handling error occurred. Details: '+E.ClassName+'/'+E.Message);
          end;
    end;

    Result := NO_ERROR;
  end;

function DllRegisterServer(): HRESULT; stdcall;
begin
  Result:=HRESULT_FROM_WIN32(NO_ERROR);
end;

function DllUnregisterServer(): HRESULT; stdcall;
begin
  Result:=HRESULT_FROM_WIN32(NO_ERROR);
end;

exports
  RadiusExtensionProcess2,
  RadiusExtensionInit,
  RadiusExtensionTerm,
  DllRegisterServer,
  DllUnregisterServer
;

begin // initialization code
{$ifdef fpc}
  Dll_Process_Detach_Hook := @Process_Detach_Hook;
  Dll_Thread_Attach_Hook := @Thread_Attach_Hook;
  Dll_Thread_Detach_Hook := @Thread_Detach_Hook;
{$else }
  Dll_Process_Attach_Hook := @DLLEntryPoint;
  Dll_Process_Detach_Hook := @DLLEntryPoint;
  //DLLProc := @DLLEntryPoint;
{$endif}
  DllEntryPoint(DLL_PROCESS_ATTACH);
end.

