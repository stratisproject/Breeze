`/api/`  

## Request/Response

RESPONSE: response code (`200` for all successful requests, `4xx`/`5xx` if error, see later)  
HEADERS: `Content-Type:application/json`  

## Errors

### General errors

BODY
The error response is an array of error objects.
Depending on the circumstance the API will either return an error at the first encounter or will continue until multiple errors are gathered.

```
{
  "errors": [
    {
      "status": 400,
      "message": "No wallet file found at Wallets\\testwallet.json",
      "description": "System.ArgumentException: No wallet file found at..."
    }
  ]
}
```  

### wallet is not created

This error message comes at all request if the wallet is not created yet, except
- `POST /wallet/create`
- `POST /wallet/recover`
- `POST /wallet/send-transaction`

```
404 (Not Found) - "wallet is not created"
```  

### wallet is not decrypted

This error message comes at all request if the wallet is not loaded yet, except
- `POST /wallet/create`
- `POST /wallet/recover`
- `POST /wallet/send-transaction`
- `POST /wallet/load`
- `DELETE /wallet`

```
400 (Bad Request) - "wallet is not decrypted"
```  

## Key Management

```
GET /wallet/general-info - Displays general information on the wallet
GET /wallet/extpubkey - Displays the extpubkey of the specified account
GET /wallet/status - Displays dynamic information on the wallet
POST /wallet/create - Creates the wallet
POST /wallet/load - Loads the wallet and starts syncing
POST /wallet/recover - Recovers the wallet
DELETE /wallet - Deletes the wallet
POST /wallet/account
POST /wallet/address
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

## GET /wallet/mnemonic - Generate a mnemonic

### Query parameters
`language`  (optional) - the language for the words in the mnemonic. Options are: English, French, Spanish, Japanese, ChineseSimplified and ChineseTraditional. The default is 'English'.  
`wordcount` (optional) - the number of words in the mnemonic. Options are: 12,15,18,21 or 24. the default is 12.  
### Examples
request
```
http://localhost:37220/api/wallet/mnemonic?wordcount=15&language=French
```
response
```
"larme essorer sabre casque gentil flamme érosion acheter caribou broder endiguer ordonner vacarme dosage défrayer"
```

request
```
http://localhost:37220/api/wallet/mnemonic?wordcount=12&language=english
```
response
```
"gravity sock glove cage divert creek mountain connect small banana depend thunder"
```


## GET /wallet/general-info - Displays general information on the wallet

### Query parameters
`name` (required) - the name of the wallet.

### Examples

#### Request
```
http://localhost:37220/api/wallet/general-info?name=testwallet
```

#### Response
```
{
  "walletFilePath":null,
  "network":"testnet", //"main", "testnet", "stratismain", "stratistest"
  "creationTime":"1511169493",
  "isDecrypted":true,
  "lastBlockSyncedHeight":1231116,
  "chainTip":1231116,
  "isChainSynced":true,
  "connectedNodes":8
}
```

## GET /wallet/extpubkey - Displays the extpubkey of the specified account
### Query parameters
`walletName` (required) - the name of the wallet.

### Examples
#### Request
```
http://localhost:37220/api/wallet/extpubkey?walletName=testwallet&accountName=account%200
```

#### Response
Returns the public key hash of the account.
```
"tpubDDVB7J4oNpyWFUVp91UcQnxUVJExWPV5NecBFTzQVH6d3A9pcrYCvu8jGzCHVAzyD99Sk3g3kLYMx6MocpzmtusmDgpbx27Msc5iCKefMUm"
```
## GET /wallet/status - Displays dynamic information on the wallet
### Responses
```
{
  "connectedNodeCount": "7",
  "maxConnextedNodeCount": "8",
  "headerChainHeight": "1048",
  "trackingHeight": "1047",
  "trackedTransactionCount": "306",
  "trackedScriptPubKeyCount": "100",
  "walletState": "syncingBlocks", // notStarted/syncingHeaders/syncingBlocks/mempoolStalling/syncingMempool/synced
  "historyChangeBump": "231321" // every time something changes this number is dumped. So it's enough to poll this get request and no need to poll the history or balances get request constantly to update the up, but only when this number changes
}
```

## POST /wallet/create - Creates the wallet
### Parameters
`name` - case-sensitive name of the wallet to be created.  
`password` - password for the wallet to be created.  
`mnemonic` (optional) - the user's mnemonic for the wallet.  
```
{
  "name": "testwallet",
  "password": "testpassword",
  "mnemonic": "gravity sock glove cage divert creek mountain connect small banana depend thunder"
}
```
### Responses
Returns the mnemonic for the wallet. If there was no mnemonic defined as input then a newly generated word list will be returned.  
```
{
  "mnemonic": "gravity sock glove cage divert creek mountain connect small banana depend thunder",
}
```
## POST /wallet/load - Loads the wallet and starts syncing
### Parameters
```
{ 
	"folderPath": "Wallets", // optional, if the folder path is not the default one
	"name": "testwallet",
	"password": "testpassword"	
}
```

### Response
```
200 (OK)
```

## POST /wallet/recover - Recovers the wallet
### Parameters
```
{
  "network": "testnet", // "main" or "testnet"
  "folderPath": "Wallets", // optional, if the folder path is not the default one
  "name": "testwallet-recovered",
  "password": "testpassword",  
  "mnemonic": "gravity sock glove cage divert creek mountain connect small banana depend thunder",
  "creationTime": "2017-02-25 16:20:33" // date from which to start looking for transactions
}
```
### Response
Cannot check if the password is good or not. If the password is wrong it'll recover a wallet with the wrong password.
```
200 (OK)
```

## DELETE /wallet - Deletes the wallet
Works as expected.

## POST /wallet/account - Gets an unused account from the wallet
This endpoint will get the first account containing no transaction or will create a new account.
### Parameters
```
{
    "walletName": "testwallet",    
    "password": "testpassword",
    "coinType": 105 // 0 - Bitcoin, 105 - Stratis
}
```
### Responses
```
  "account one"
```

## GET /wallet/address - Gets an unused address 

This endpoint will get the last address containing no transaction or will create a new address.
### Query parameters
`walletName` (required) - the name of the wallet in which this address is contained.  
`coinType` (required) - the type of coin for which to get the address, e.g 0 for bitcoin, 105 for stratis.  
`accountName` (required) - the name of the account in which this address is contained.  
### Responses
```
  "1HDypWxXWZC5KXK259EHMnrWaa2youy7Mj"
```

## GET /wallet/receive/[account1/account2] - Displays unused receive addresses of the specified wallet account
### Responses
```
{
  "addresses": // 7 unused receive address (7 is the best number: https://www.psychologytoday.com/blog/fulfillment-any-age/201109/7-reasons-we-7-reasons)
  [
    "mzz63n3n89KVeHQXRqJEVsQX8MZj5zeqCw",
    "mhm1pFe2hH7yqkdQhwbBQ8qLnMZqfL6jXb",
    "mmRzqMDBrfNxMfryQSYec3rfPHXURNapBA",
    "my2ELDBqLGVz1ER7CMynDqG4BUpV2pwfR5",
    "mmwccp4GefhPn4P6Mui6DGLGzHTVyQ12tD",
    "miTedyDXJAz6GYMRasiJk9M3ibnGnb99M1",
    "mrsb39MmPceSPfKAURTH23hYgLRH1M1Uhg"
  ]
}
```

## GET /wallet/history - Displays the history of the specified wallet account
### Query parameters
`walletName` (required) - the name of the wallet.  
`coinType` (required) - the type of coin, e.g 0 for bitcoin, 105 for stratis.  
### Responses
```
{
  "transactionsHistory": [    
    {
      "type": "send",
      "id": "6358161c713688e372481fce7f20f3f8692ab2e4e657f3d9afa750ebee54e6c3",
      "amount": 500000,
      "payments": [
        {
          "destinationAddress": "mt7W2Zf69KC9472TPCzUeLLhBDSmC82AWz",
          "amount": 500000
        }
      ],
      "fee": 100000,
      "confirmedInBlock": 1122310,
      "timestamp": "1494594937"
    },
    {
      "type": "received",
      "toAddress": "mnDsG7kTYCeVNqnEmfvdYeNgZwxhjqm2jc",
      "id": "75ce74643aae01ccbe2bbc05efb4788cc9a16a9192add4d7082561a40a541057",
      "amount": 110000000,
      "confirmedInBlock": 1122292,
      "timestamp": "1494591670"
    }
  ]
}
```

## GET /wallet/balance - Displays the balances of the specified wallet account
### Query parameters
`walletName` (required) - the name of the wallet.  
`coinType` (required) - the type of coin, e.g 0 for bitcoin, 105 for stratis.  

### Examples
#### Request
```
http://localhost:37220/api/wallet/balance?walletName=testwallet
```
#### Response
```
{
  "balances": [
    {
      "accountName": "account one",
      "accountHdPath": "m/44'/0'/0'",
      "coinType": 0,
      "amountConfirmed": 209268016,
      "amountUnconfirmed": 0
    }
  ]
}
```
Confirmed balance is the (amount of unspent confirmed outputs - unconfirmed outgoing transactions). It cannot be negative.  
Unconfirmed balance is the difference of unconfirmed incoming and outgoing transactions. It can be negative.  

## POST /wallet/build-transaction/ - Attempts to build a transaction with the specified wallet account
### Parameters
```
{
  "walletName": "testwallet",    
  "accountName": "account 0",      
  "coinType": 0,
  "password": "password",
  "destinationAddress": "1Xyz...",
  "amount": "0.12", // in btc, if 0, then spends all available
  "feeType": "low", // "low"/"medium"/"high"
  "allowUnconfirmed": true // if spending unconfirmed outputs is allowed
}
```

### Responses
#### Successful
```
{
    "fee": "0.0001",
    "hex": "0100000002d9dced2b6fc80c706d3564670cb6706afe7a798863a9218efcdcf415d58f0f82000000006a473044022030b8bea478444bd52f08de33b082cde1176d3137111f506eefefa91b47b1f6bf02204f12746abd1aeac5805872d163592cf145967fa0619339a9c5348d674852ef4801210224ec1e4c270ce373e6999eebfa01d0a7e7db3c537c026f265233350d5aab81fbfeffffffa0706db65c5e3594d43df5a2a8b6dfd3c9ee506b678f8c26f7820b324b26aa0f000000006a473044022061b718034f876590d6d80bac77a63248b2548d934849acd02c4f4236309e853002201aded6b24f553b6902cf571276b37b12f76b75650164d8738c74469b4edd547e012103d649294a0ca4db920a69eacd6a75cb8a38ae1b81129900621ce45e6ba3438a7bfeffffff0280a90300000000001976a914d0965947ebb329b776328624ebde8f8b32dc639788ac1cc80f00000000001976a914c2a420d34fc86cff932b8c3191549a0ddfd2b0d088acba770f00",
    "transactionId": "86f348bce07b04b2f7a00d882349e66d98765e935484516ce5fca97685566155"
}
```

#### Errors
```
400 - "wallet is not synced"
```  

## POST /wallet/send-transaction - Attempts to send a transaction
### Parameters
```
{
  "hex": "01000000061b1ca819e76f9131b23335ec905ffc5fc27e36a7843a5b7c6d1b455b904359f7000000006b483045022100c11f78ce7f02b2312b6675d3ad99cec6ede879d446c2b14628ef4f8ce9b3fdc5022073649a14971568a1cd2aa84b5dd404645f29e49882f60a9642850539443872fe012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff1d3c389af5fdd047e307e5d5f87656bb2ef0c40b6ee879d342a59192090d3fbc000000006b483045022100dc7e0445fe98f3e76d68906c640ecca597598a03b48e6b85d72918347b9da7330220340ce9e9533ea84375a1f2122b7868b8ab556da53f1e1af14d1a71b0b123aade012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff1ed624bad3df9d3a7be56dcd5d97c996fccc78164f16f59658b33f8da8859deb000000006b483045022100ba2a55f55a37b6712dd25dbef411aed869190ef60a208b39d4bd8e0ce8635b4d02201976d63489e23205aab651a9def43d6b3a740ba06de2ecabc43504241a71f229012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff24e5cb4893beb0bb60193dbe11a9778d07e127c1cbc939c0a0388b1013ef75d9000000006a47304402203c27eea34db0ba070bee38d625d2cfcec1a0f5d8a9124023c84e9963d37f6145022015f7657cc57be515e6aa43c93c73457f5583b7c90c0af4e8a2f913257df27b0b012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff997e6738c45eaf7af8bed7dc09e258139bffb0d2be8b4167473b6943adc0b28b000000006a47304402202d4c6df39b725d571d67bef14f0c6baa0cf4b93aa54aac2d2a15d3d940510d0602203643162545d5b63c007986627a317ed962f4d5023e4c15e9636a4eede86930c7012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffffcff2021b6b0bcd2a8b38583539dc140b98da4f41ae1e4adb089dc2cf3b66d6c6000000006a473044022019d5264c99145c7203e690fb2f57b0e218af2761e024f9ec1b774c703939b96e02204c2430fc4ae0fa43afb19a722f7b5d706bf5f2d5ee85229cbdc7a7b26433f5fd012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff018f73e606000000001976a914ec093b0943ec524769553e1b7261b67ecab47e8688ac00000000"
}
```
