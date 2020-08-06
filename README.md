# block_io-csharp
# BlockIo

This C# library is the official reference client for the Block.io payments API and uses .NET core version 3.1. To use this, you will need the Dogecoin, Bitcoin, or Litecoin API key(s) from <a href="https://block.io" target="_blank">Block.io</a>. Go ahead, sign up :)

## Installation

1. Clone the repo
2. dotnet restore
3. dotnet run --project ./BlockIoLib

## Usage

It's super easy to get started. In your code, do this:

    BlockIo blockLib = new BlockIo(API_KEY, PIN, VERSION);

    // to pass options:

    BlockIo blockLib = new BlockIo(API_KEY, PIN, VERSION, new Options("API URL", "Bool to allow no pin"))

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
        new {
            pin="secret_pin",
            from_labels="label1, label2",
            to_label="label3",
            amount="50.0"
        }).Data;   

##### A note on passing json args to requests:

Args are passed as objects like this: 

    new { param1="string", param2="intVal", param3="this,is,a,list"}

## Testing

We use NUnit for unit tests that ensure all internal library functions work correctly.

**DO NOT USE PRODUCTION CREDENTIALS FOR UNIT TESTING!** 

Test syntax:

```bash
dotnet test
