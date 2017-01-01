#!/usr/bin/env python
# $Id$

import socket
import radiusd
from xml.dom.minidom import parseString

def instantiate(p):
  radiusd.radlog(radiusd.L_INFO, 'sms2.instantiate')
  return 0

def authorize(authData):
  radiusd.radlog(radiusd.L_INFO, 'sms2.authorize')
  userName = ''
  userPasswd = ''

  foundAttr = 0
  for t in authData:
    if t[0] == 'User-Name':
      userName = t[1]
      foundAttr = 1
    elif t[0] == 'User-Password':
      userPasswd = t[1]
      foundAttr = 1

  if foundAttr == 0:
    return radiusd.RLM_MODULE_OK


  authUser = "<AuthEngineRequest><Commands><ValidateUser><User>{0}</User><PinCode>{1}</PinCode></ValidateUser></Commands></AuthEngineRequest>"
  #authUserPin = '<AuthEngineRequest><Commands><ValidatePin><User>username</User><Pin>pin</Pin><PinCode>pincode</PinCode><State>state</State></ValidatePin></Commands></AuthEngineRequest>'

  TCP_IP = '192.168.1.145'
  TCP_PORT = 9060
  sms2cmd = authUser.format(userName[1:-1], userPasswd[1:-1])
  sms2resp = ''
  try:
    sock = socket.create_connection((TCP_IP, TCP_PORT))

    radiusd.radlog(radiusd.L_DBG, 'sms2.authorize.send: {0}\n'.format(sms2cmd))

    sock.sendall(sms2cmd)

    data = sock.recv(4096)
    while len(data):
      sms2resp = sms2resp + data
      data = sock.recv(4096)
    sock.close()
    sock = None

    radiusd.radlog(radiusd.L_DBG, 'sms2.authorize.recv: {0}\n'.format(sms2resp))
  except socket.error, msg:
    radiusd.radlog(radiusd.L_ERR, 'sms2.[ERROR] {0}\n'.format(msg[1]))
    print "[ERROR] {0}\n".format(msg[1])
    return radiusd.RLM_MODULE_OK
  except:
    radiusd.radlog(radiusd.L_ERR, 'sms2.[ERROR] unknown\n')
    print "[ERROR] unknown\n"
    return radiusd.RLM_MODULE_OK

  if not len(sms2resp):
    return radiusd.RLM_MODULE_OK

  def getNodeText(node):
    if len(node) == 0:
      return ''
    return ''.join(t.nodeValue for t in node[0].childNodes if t.nodeType == t.TEXT_NODE)
  sms2dom = parseString(sms2resp)
  vur_error = getNodeText(sms2dom.getElementsByTagName('Error'))
  vur_validated = getNodeText(sms2dom.getElementsByTagName('Validated'))

  if vur_error:
    return radiusd.RLM_MODULE_REJECT

  if vur_validated == 'true':
    return (radiusd.RLM_MODULE_UPDATED,
      #(('Session-Timeout', 'hola'),),
	  (),
      (('Auth-Type', 'python'),))

  return radiusd.RLM_MODULE_REJECT

def authenticate(p):
  radiusd.radlog(radiusd.L_INFO, 'sms2.authenticate')
  return radiusd.RLM_MODULE_OK

def accounting(p):
  radiusd.radlog(radiusd.L_INFO, 'sms2.accounting')
  return radiusd.RLM_MODULE_OK

# Test
if __name__ == '__main__':
  instantiate(None)
  print authorize((('User-Name', '"test"'), ('User-Password', '"test"')))

