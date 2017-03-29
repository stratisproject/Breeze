`/api/v1/`  
  
## Request/Response

RESPONSE: responsecode (`200` if success, `400`/`500` if error, see later)  
  
HEADERS
`Content-Type:application/json`  
  
## Errors

`400` series status codes for client issues & `500` series status codes for server issues.  
API should standardize that all `400` series errors come with consumable JSON error representation.  
  
BODY  
```
{
  "success" : "false",
  "message" : "something bad happened", // ex.Message maybe?
  "description" : ex.ToString()
}
```  

### wallet is not created

This error message comes at all request if the wallet is not created yet, except
- `POST /wallet/create`
- `POST /wallet/recover`
- `POST /wallet/send-transaction`

```
{
  "success" : "false",
  "message" : "wallet is not created",
  "description" : ""
}
```  

### wallet is not decrypted

This error message comes at all request if the wallet is not loaded yet, except
- `POST /wallet/create`
- `POST /wallet/recover`
- `POST /wallet/send-transaction`
- `POST /wallet/load`
- `DELETE /wallet`

```
{
  "success" : "false",
  "message" : "wallet is not decrypted",
  "description" : ""
}
```  

## Key Management

```
GET /wallet/general - Displays general information on the wallet 
GET /wallet/sensitive - Displays sensitive information on the wallet 
GET /wallet/status - Displays dynamic information on the wallet 
POST /wallet/create - Creates the wallet
POST /wallet/load - Loads the wallet and starts syncing
POST /wallet/recover - Recovers the wallet
DELETE /wallet - Deletes the wallet
```
	
## Syncing

```
GET /wallet/mempool/?allow=[true/false] - Allows or disallows mempool syncing
```

## Key Monitoring

```
GET /wallet/receive/[account1/account2] - Displays unused receive addresses of the specified wallet account
GET /wallet/history/[account1/account2] - Displays the history of the specified wallet account
GET /wallet/balance/[account1/account2] - Displays the balances of the specified wallet account
```

## Transaction building and sending

```
POST /wallet/build-transaction/[account1/account2] - Attempts to build a transaction with the specified wallet account
POST /wallet/send-transaction - Attempts to send a transaction
```
