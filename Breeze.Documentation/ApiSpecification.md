`/api/v1/`  
  
## Request/Response

RESPONSE: responsecode (`200` if success, `400`/`500` if error, see later)  
```
{
  "success": "true"
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
  "success": "false",
  "message": "something bad happened", // ex.Message maybe?
  "description": ex.ToString()
}
```  

### wallet is not created

This error message comes at all request if the wallet is not created yet, except
- `POST /wallet/create`
- `POST /wallet/recover`
- `POST /wallet/send-transaction`

```
{
  "success": "false",
  "message": "wallet is not created",
  "description": ""
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
  "success": "false",
  "message": "wallet is not decrypted",
  "description": ""
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
### Responses
```
{
  "success": "true",
  "walletFilePath": "path to the wallet file",
  "encryptedSeed": "6PYKWP34en1wELfcJDgXaFRPugjgkDdEk2p2Pzytm1158dxgNyLAUXwpKL",
  "chainCode": "q/Fn7+RSIVM0p0Nj6rIuNkybF+0WKeSZPMQS2QCbDzY=",
  "network": "main", // main/testnet
  "creationTime": "2017-03-21",
  "decrypted": "true",
  "uniqueId": "sadwpiqjdpijwqdpijwqidjoi" // can only get if decrypted, if not it's empty string
}
```
## GET /wallet/sensitive - Displays sensitive information on the wallet
### Parameters
```
{
  "password": "password"  
}
```
### Responses
```
{
  "success": "true",
  "extkey": "sadwqdpqoijedqcdoijsadoijsadisa",
  "extpubkey": "dalkdsaklkjdlkjdsaljlkjdsalkjdsalk",
}
```
## GET /wallet/status - Displays dynamic information on the wallet
## POST /wallet/create - Creates the wallet
### Parameters
```
{
  "network": "main", // "main" or "testnet"
  "password": "password"  
}
```
### Responses
```
{
  "success": "true",
  "mnemonic": "foo bar buz",
}
```
## POST /wallet/load - Loads the wallet and starts syncing
### Parameters
```
{
  "password": "password"
}
```
## POST /wallet/recover - Recovers the wallet
### Parameters
```
{
  "network": "main", // "main" or "testnet"
  "password": "password",  
  "mnemonic": "foo bar buz",
  "creationTime": "2017-02-03" // DateTimeOffset.ParseExact("1998-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture), utc time
}
```
### Response
Cannot check if the password is good or not. If the password is wrong it'll recover a wallet with the wrong password.

## DELETE /wallet - Deletes the wallet
Works as expected.

## GET /wallet/mempool/?allow=[true/false] - Allows or disallows mempool syncing
Works as expected.

## GET /wallet/receive/[account1/account2] - Displays unused receive addresses of the specified wallet account
### Responses
```
{
  "success": "true",
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

## GET /wallet/history/[account1/account2] - Displays the history of the specified wallet account
### Responses
```
{
  "success": "true",
  "history": 
  [
    {
      "txid": "9a9949476b629b4075b31d8faad64dad352586a18df8f2810c5a7bb900478c60",
      "amount": "0.1",
      "confirmed": "true",
      "timestamp": "2016.12.19. 23:15:05" // if confirmed it's the blocktime, utc
    },
    {
      "txid": "9a9949476b629b4075b31d8faad64dad352586a18df8f2810c5a7bb900478c60",
      "amount": "-0.1",
      "confirmed": "false",
      "timestamp": "2016.12.20. 1:15:36" // if unconfirmed it's the time our node first seen this transaction, utc
    }
  ]
}
```

## GET /wallet/balance/[account1/account2] - Displays the balances of the specified wallet account
### Responses
```
{
  "success": "true",
  "synced": "true",
  "confirmed": "0.144",
  "unconfirmed": "-6.23"
}
```
If the synced is false, then the balances might not be accurate.  
Confirmed balance is the (amount of unspent confirmed outputs - unconfirmed outgoing transactions). It cannot be negative.  
Unconfirmed balance is the difference of unconfirmed incoming and outgoing transactions. It can be negative.  

## POST /wallet/build-transaction/[account1/account2] - Attempts to build a transaction with the specified wallet account
### Parameters
```
{
  "password": "password",
  "address": "1Xyz...",
  "amount": "0.12", // in btc, if 0, then spends all available
  "feeType": "low", // "low"/"medium"/"high"
  "allowUnconfirmed": "true" // if spending unconfirmed outputs is allowed
}
``` 

### Responses
#### Successful
```
{
	"success": "true",
	"spendsUnconfirmed": "false", // If spends unconfirmed you can ask the user if it's sure about spending unconfirmed transaction (if inputs are malleated or inputs never confirm then this transaction will never confirm either" 
	"fee": "0.0001",
	"feePercentOfSent": "0.1" // Percentage of the total spent amount, there must be a safety limit implemented here
	"hex": "0100000002d9dced2b6fc80c706d3564670cb6706afe7a798863a9218efcdcf415d58f0f82000000006a473044022030b8bea478444bd52f08de33b082cde1176d3137111f506eefefa91b47b1f6bf02204f12746abd1aeac5805872d163592cf145967fa0619339a9c5348d674852ef4801210224ec1e4c270ce373e6999eebfa01d0a7e7db3c537c026f265233350d5aab81fbfeffffffa0706db65c5e3594d43df5a2a8b6dfd3c9ee506b678f8c26f7820b324b26aa0f000000006a473044022061b718034f876590d6d80bac77a63248b2548d934849acd02c4f4236309e853002201aded6b24f553b6902cf571276b37b12f76b75650164d8738c74469b4edd547e012103d649294a0ca4db920a69eacd6a75cb8a38ae1b81129900621ce45e6ba3438a7bfeffffff0280a90300000000001976a914d0965947ebb329b776328624ebde8f8b32dc639788ac1cc80f00000000001976a914c2a420d34fc86cff932b8c3191549a0ddfd2b0d088acba770f00"
	"transaction": // NBitcoin.Transaction.ToString()
	{
	  "transaction": "0100000002d9dced2b6fc80c706d3564670cb6706afe7a798863a9218efcdcf415d58f0f82000000006a473044022030b8bea478444bd52f08de33b082cde1176d3137111f506eefefa91b47b1f6bf02204f12746abd1aeac5805872d163592cf145967fa0619339a9c5348d674852ef4801210224ec1e4c270ce373e6999eebfa01d0a7e7db3c537c026f265233350d5aab81fbfeffffffa0706db65c5e3594d43df5a2a8b6dfd3c9ee506b678f8c26f7820b324b26aa0f000000006a473044022061b718034f876590d6d80bac77a63248b2548d934849acd02c4f4236309e853002201aded6b24f553b6902cf571276b37b12f76b75650164d8738c74469b4edd547e012103d649294a0ca4db920a69eacd6a75cb8a38ae1b81129900621ce45e6ba3438a7bfeffffff0280a90300000000001976a914d0965947ebb329b776328624ebde8f8b32dc639788ac1cc80f00000000001976a914c2a420d34fc86cff932b8c3191549a0ddfd2b0d088acba770f00",
	  "transactionId": "22ab5e9b703c0d4cb6023e3a1622b493adc8f83a79771c83a73dfa38ef35b07c",
	  "isCoinbase": false,
	  "block": null,
	  "spentCoins": [
		{
		  "transactionId": "820f8fd515f4dcfc8e21a96388797afe6a70b60c6764356d700cc86f2beddcd9",
		  "index": 0,
		  "value": 100000,
		  "scriptPubKey": "76a914e7c1345fc8f87c68170b3aa798a956c2fe6a9eff88ac",
		  "redeemScript": null
		},
		{
		  "transactionId": "0faa264b320b82f7268c8f676b50eec9d3dfb6a8a2f53dd494355e5cb66d70a0",
		  "index": 0,
		  "value": 1180443,
		  "scriptPubKey": "76a914f3821cff5a90328271d8596198f68e97fbe2ea0e88ac",
		  "redeemScript": null
		}
	  ],
	  "receivedCoins": [
		{
		  "transactionId": "22ab5e9b703c0d4cb6023e3a1622b493adc8f83a79771c83a73dfa38ef35b07c",
		  "index": 0,
		  "value": 240000,
		  "scriptPubKey": "76a914d0965947ebb329b776328624ebde8f8b32dc639788ac",
		  "redeemScript": null
		},
		{
		  "transactionId": "22ab5e9b703c0d4cb6023e3a1622b493adc8f83a79771c83a73dfa38ef35b07c",
		  "index": 1,
		  "value": 1034268,
		  "scriptPubKey": "76a914c2a420d34fc86cff932b8c3191549a0ddfd2b0d088ac",
		  "redeemScript": null
		}
	  ],
	  "firstSeen": "2016-10-31T09:13:18.4420023+00:00",
	  "fees": 6175
	}
}
```

#### Errors
```
{
  "success": "false",
  "message": "wallet is not synced",
  "description": ""
}
```  

## POST /wallet/send-transaction - Attempts to send a transaction
### Parameters
```
{
  "hex": "01000000061b1ca819e76f9131b23335ec905ffc5fc27e36a7843a5b7c6d1b455b904359f7000000006b483045022100c11f78ce7f02b2312b6675d3ad99cec6ede879d446c2b14628ef4f8ce9b3fdc5022073649a14971568a1cd2aa84b5dd404645f29e49882f60a9642850539443872fe012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff1d3c389af5fdd047e307e5d5f87656bb2ef0c40b6ee879d342a59192090d3fbc000000006b483045022100dc7e0445fe98f3e76d68906c640ecca597598a03b48e6b85d72918347b9da7330220340ce9e9533ea84375a1f2122b7868b8ab556da53f1e1af14d1a71b0b123aade012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff1ed624bad3df9d3a7be56dcd5d97c996fccc78164f16f59658b33f8da8859deb000000006b483045022100ba2a55f55a37b6712dd25dbef411aed869190ef60a208b39d4bd8e0ce8635b4d02201976d63489e23205aab651a9def43d6b3a740ba06de2ecabc43504241a71f229012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff24e5cb4893beb0bb60193dbe11a9778d07e127c1cbc939c0a0388b1013ef75d9000000006a47304402203c27eea34db0ba070bee38d625d2cfcec1a0f5d8a9124023c84e9963d37f6145022015f7657cc57be515e6aa43c93c73457f5583b7c90c0af4e8a2f913257df27b0b012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff997e6738c45eaf7af8bed7dc09e258139bffb0d2be8b4167473b6943adc0b28b000000006a47304402202d4c6df39b725d571d67bef14f0c6baa0cf4b93aa54aac2d2a15d3d940510d0602203643162545d5b63c007986627a317ed962f4d5023e4c15e9636a4eede86930c7012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffffcff2021b6b0bcd2a8b38583539dc140b98da4f41ae1e4adb089dc2cf3b66d6c6000000006a473044022019d5264c99145c7203e690fb2f57b0e218af2761e024f9ec1b774c703939b96e02204c2430fc4ae0fa43afb19a722f7b5d706bf5f2d5ee85229cbdc7a7b26433f5fd012102a41e4348bb233e40cf3a3402e2dc92a31b69ef56090fff242aa7e4bff828929fffffffff018f73e606000000001976a914ec093b0943ec524769553e1b7261b67ecab47e8688ac00000000"
}
```
