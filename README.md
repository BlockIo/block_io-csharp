# block_io-csharp
# BlockIo

This C# library is the official reference client for the Block.io payments API and uses .NET core version 3. To use this, you will need the Dogecoin, Bitcoin, or Litecoin API key(s) from <a href="https://block.io" target="_blank">Block.io</a>. Go ahead, sign up :)

## Usage

It's super easy to get started. In your code, do this:

    BlockIo blockLib = new BlockIo(API_KEY, PIN);

    // print the account balance request's response
    var data = blockLib.GetBalance().Data;

    // print the request's status
    var status = blockLib.GetBalance().Status

    // print all addresses on this account
    var data = blockLib.GetMyAddresses().Data;

    // print the response of a withdrawal request
    // 'SECRET_PIN' is only required if you did not specify it at 
    // class initialization time.
    var data = blockLib.Withdraw(
    "{ 
        pin: 'SECRET_PIN', 
        from_labels: 'label1,label2', 
        to_label: 'label3', 
        amount: '50.0' 
    }").Data;   

##### A note on passing json args to requests:

Args are in JSON format and need to be passed like this: 

    "{param1: 'stringValue', param2: intValue, param3: ['this', 'is', 'a', 'list', 'of', 'strings']}"

## Testing

We use NUnit for unit tests that ensure all internal library functions work correctly.

**DO NOT USE PRODUCTION CREDENTIALS FOR UNIT TESTING!** 

Test syntax:

```bash
dotnet test
