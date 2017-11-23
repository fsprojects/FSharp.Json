Supported Types
===============

Here's full list of F# types that are supported by FSharp.Json library.

int
---
Represented as `number` in JSON.

int64
----- 
Represented as `number` in JSON.

float
-----
Represented as `number` in JSON.

decimal
-------
Represented as `number` in JSON.

string
------
Represented as `string` in JSON.

bool
----
Represented as `bool` in JSON.

DateTime
--------
Represented as `string` according to [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601).
Could be represented as `number` epoch time using [transform](transform.html).

DateTimeOffset
--------------
Represented as `string` according to [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601).
Could be represented as `number` epoch time using [transform](transform.html).

Guid
----
Represented as `string`.

Enum
----
Represented as `string` that is enum value name.
Could be represented as `number` corresponding to enum value.
Read more on [Enums](enums.html) page.

option
------
Option is not represented by itself.
None value might be represented as `null` or omitted.
Read more on [Null Safety](null_safety.html) page.

tuple
-----
Tuples are represented as `list` in JSON.

record
------
Records are represented as `object` in JSON.

map
---
Map is represented as `object` in JSON.

array
-----
Array is represented as `list` in JSON.

list
----
List is represented as `list` in JSON.

union
-----
Unions are represented as `object` with special structure in JSON.
Read more on [Unions](unions.html) page.
