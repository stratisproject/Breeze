`/api/v1/`  
  
## Request/Response

RESPONSE: responsecode (`200` if success, `400`/`500` if error, see later)  
```
{
  success: "true"
}
```
  
HEADERS
`Content-Type:application/json`  
  
## Errors

### General errors

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

# Details

## GET /wallet/general - Displays general information on the wallet
## GET /wallet/sensitive - Displays sensitive information on the wallet
## GET /wallet/status - Displays dynamic information on the wallet
## POST /wallet/create - Creates the wallet
### Parameters
```
{
  network : "network",
  password : "password"  
}
```
### Response
```
{
  success : "true",
  mnemonic : "foo bar buz",
}
```
## POST /wallet/load - Loads the wallet and starts syncing
### Parameters
```
{
  password : "password"
}
```
## POST /wallet/recover - Recovers the wallet
### Parameters
```
{
  network : "network",
  password : "password",  
  mnemonic : "foo bar buz",
  creationTime : "2017-02-03" // DateTimeOffset.ParseExact("1998-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture), utc time
}
```
### Response
Cannot check if the password is good or not. If the password is wrong it'll recover a wallet with the wrong password.

## DELETE /wallet - Deletes the wallet
Works as expected.

## GET /wallet/mempool/?allow=[true/false] - Allows or disallows mempool syncing
Works as expected.

## GET /wallet/receive/[account1/account2] - Displays unused receive addresses of the specified wallet account
## GET /wallet/history/[account1/account2] - Displays the history of the specified wallet account
## GET /wallet/balance/[account1/account2] - Displays the balances of the specified wallet account
## POST /wallet/build-transaction/[account1/account2] - Attempts to build a transaction with the specified wallet account

## POST /wallet/send-transaction - Attempts to send a transaction
### Parameters
```
{
  hex: "01000000061b1ca819e76f9131b23335ec905ffc5fc27e36a7843a5b7c6d1b455b904359f7000000006b483045022100c11f78ce7f02b2312b6675d3ad99cec6ede879d446c2b14628ef4f8ce9b3fdc5022073649a14971568a1cd2aa84b5dd404645f29e49882f60a9642850539443872fe012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff1d3c389af5fdd047e307e5d5f87656bb2ef0c40b6ee879d342a59192090d3fbc000000006b483045022100dc7e0445fe98f3e76d68906c640ecca597598a03b48e6b85d72918347b9da7330220340ce9e9533ea84375a1f2122b7868b8ab556da53f1e1af14d1a71b0b123aade012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff1ed624bad3df9d3a7be56dcd5d97c996fccc78164f16f59658b33f8da8859deb000000006b483045022100ba2a55f55a37b6712dd25dbef411aed869190ef60a208b39d4bd8e0ce8635b4d02201976d63489e23205aab651a9def43d6b3a740ba06de2ecabc43504241a71f229012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff24e5cb4893beb0bb60193dbe11a9778d07e127c1cbc939c0a0388b1013ef75d9000000006a47304402203c27eea34db0ba070bee38d625d2cfcec1a0f5d8a9124023c84e9963d37f6145022015f7657cc57be515e6aa43c93c73457f5583b7c90c0af4e8a2f913257df27b0b012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff997e6738c45eaf7af8bed7dc09e258139bffb0d2be8b4167473b6943adc0b28b000000006a47304402202d4c6df39b725d571d67bef14f0c6baa0cf4b93aa54aac2d2a15d3d940510d0602203643162545d5b63c007986627a317ed962f4d5023e4c15e9636a4eede86930c7012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffffcff2021b6b0bcd2a8b38583539dc140b98da4f41ae1e4adb089dc2cf3b66d6c6000000006a473044022019d5264c99145c7203e690fb2f57b0e218af2761e024f9ec1b774c703939b96e02204c2430fc4ae0fa43afb19a722f7b5d706bf5f2d5ee85229cbdc7a7b26433f5fd012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff018f73e606000000001976a914ec093b0943ec524769553e1b7261b67ecab47e8688ac00000000"
}
```
