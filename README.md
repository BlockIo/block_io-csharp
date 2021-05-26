# BlockIo

This repository contains the official C# SDK for the Block.io payments API
and uses .NET Core version 3.1. To use this, you will need the Bitcoin,
Litecoin or Dogecoin API key(s)
from <a href="https://block.io" target="_blank">Block.io</a>.
Go ahead, sign up :)

## Installation

    $ nuget install BlockIo

or

    > dotnet add package BlockIo  


## Usage

It's super easy to get started. In your code, do this:

```csharp
  BlockIo blockIo = new BlockIo(API_KEY);

  // print the account balance request's response
  var data = blockIo.GetBalance().Data;

  // print the request's status code
  var status = blockIo.GetBalance().Status;

  // print all addresses on this account
  var data = blockIo.GetMyAddresses().Data;

```

##### A note on passing payload to requests:

Arguments are passed as object literals, like so:

```csharp
  new { string="string", decimal="3.0000000", liststring="this,is,a,list"}
```

## Testing

We use NUnit for unit tests that ensure all internal library functions work
correctly.

Test syntax:

```bash
  dotnet test
```
