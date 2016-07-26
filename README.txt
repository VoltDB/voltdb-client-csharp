VoltDB C# Client Library
========================

The VoltDB client library implements the native VoltDB wire
protocol. You can use the library to connect to a VoltDB cluster,
invoke stored procedures and read responses.

For an introduction to using the C# library:
[Introducting VoltDB.NET: C# Library for Your VoltDB Applications](https://voltdb.com/blog/introducing-voltdbnet-c-library-your-voltdb-applications)

## Getting Started ##

Download the driver:
- Latest build with documentation: [Downloads](https://downloads.voltdb.com/loader.php?kit=CsharpClient)
- Full source on Github: [voltdb-client-csharp](https://github.com/VoltDB/voltdb-client-csharp)

## VoltDB.NET in a nutshell ##

The .NET/C# client library for VoltDB is extremely flexible and allows you to develop WinForms, Console and Web applications and services much as you would leveraging any other back-end data service or database.

- Connect with a customized connectionstring in your app/web.config.
- Perform Synchronous or Asynchronous data operations using standard .NET design patterns.
- Consume and filter data results using LINQ.
- Leverage native, strongly-typed, data access and type casting, as well as late-bound operations.
- Monitor performance and manage a database cluster.

Key considerations:

- All access is thread-safe
    - Safe for multi-threaded application
    - Safe as shared connections for a website/service
    - OK to mix asynchronous and synchronous calls
    - OK to have long-running callbacks (they are Thread-Pooled)
- Resilience
    - Connect to multiple nodes
- Performance
    - Share: multi-threading on a single connection is faster
    - Production: No Tracing (ever) or Statistics (unless needed)
    - Avoid usage of IAsync WaitHandles: they are slow!


## Connecting to VoltDB ##

Read the section "Creating a Database Connection" of blog post: [Introducting VoltDB.NET: C# Library for Your VoltDB Applications](https://voltdb.com/blog/introducing-voltdbnet-c-library-your-voltdb-applications).


Key Connection Options:
- Port – default: 21212
- MaxOutstandingTransaction (txnq) – default: 3,000
- Statistics (stats) – default: false
- CommandTimeout – default: 5,000(ms)
- AllowSystemCalls (system) – default: false
- AllowAdHocQueries (adhoc) – default: false

## Synchronous and Asynchronous Calls ##

Synchronous = Do nothing until you get the answer
- Good: ‘Procedural’ model easier to understand/work with
- Bad: Limits your throughput
- Redeeming: For this thread only

Asynchronous = Call me when you get the answer
- Good: No bottlenecks on wait operations
- Bad: ‘Disjointed’ model harder to work with/error prone
- Redeeming: Known ‘Best Practice’ patterns

VoltDB.NET implements:
- Sync: .Execute
- Async: .Begin/.Cancel/.End with IAsyncResult

# Example Code #

## Calling Procedures ##

Define Callback for Asynchronous call

    void MyDelegate(Response<Table[]> response) {
      if (response.Status == ResponseStatus.Success) {
        … // Send response to client, update UI, etc.
      }
      else {
        … // Deal with error
      }
    }

Create Strongly-typed Wrappers

    var my = conn.Procedures.Wrap<Table[],int,string>("MyProcedure", MyDelegate);

Supported Data Types:

* Result:
    - Table[], Table
    - SingleRowTable[], SingleRowTable
    - int[], long[], string[], double[], … (and nullable types)
    - int, long, … (and nullable types)
* Parameters:
    - int[], long[], string[], double[], …
    - int, long, …

Execute the procedure

    Response<Table[]> r = myProc.Execute(1, “test”);

    IAsyncResult h = my.BeginExecute(1, “test”);
    IAsyncResult h = my.BeginExecute(1, “test”, state);

Cancel Async Execution

    myProc.ExecuteCancelAsync(h);

Get Async results (if not using callback)

    Response<Table[]> r = myProc.EndExecute(h);

Wrapper Rules:

- Up to 35 input parameters
- Types must be compatible with core VoltDB types
- Types can be single-values or arrays
    - sbyte, short, int, long, double, VoltDecimal, DateTime, string
    - sbyte?, short?, int?, …
    - sbyte[], sbyte?[], short[], short?[], …


Another option is to use Runtime Wrappers, but the type of runtime values must still be compatible.

    conn.Procedures.Wrap<Table[],object,…,object>(…);

Per-execution callback delegate/closures

    IAsyncResult h = my.BeginExecute(1, "test", MyDelegate);
    
    // or...
    
    h = my.BeginExecute( 1, "test", (r) => MyClosureFunction(r, …) );

Re-use Wrappers across connections (of course, executions still occur in the initiating context/connection).

    my.SetConnection(otherVoltConnection);



## Consuming Results ##

Access data directly...

    double? value = response.Result
                            .GetValue<double?>(col, row);

    double? Value = response.Result
                            .Rows
                            .ElementAt(row)
                            .GetValue<double?>(col);

...or through Strongly-typed Table Wrappers

    var myTable = response.Result.Wrap<int?,…,double?>();

    double? value = myTable.Column7[row];

Wrapper Rules:

- Up to 35 columns
- Types must be compatible with core VoltDB types and flagged as Nullable => use int? (not int)
    - sbyte?, short?, int?, long?, double?, VoltDecimal?, DateTime? and string

Results are LINQ-friendly:

    // On a strongly-typed VoltDB data table
    myTable.Rows.Where(r => r.Column2 == "Books")
                .Select(r => new { Title = r.Column2, Price = r.Column7 })
                .OrderBy(p => p.Price);

    // On a raw VoltDB data table
    raw.Rows.Where(r => r.GetValue<string>(1) == "Books")
            .Select(r => new { Title = r. GetValue<string>(1), Price = r. GetValue<double>(6) })
            .OrderBy(p => p.Price);

Access metadata:

    int count = myTable.RowCount;
    bool check = myTable.HasData;

    string name = raw.GetColumnName(idx);
    short idx = raw.GetColumnIndex(name);

    Type type = raw.GetColumnType(idx);
    DBType type = raw.GetColumnDBType(idx);

    int?[] column1Data = raw.GetColumnData<int?>(0);
    object[] column1Data = raw.GetColumnData(0);

Fill a (System.Data.)DataTable

    Table raw = procedureWrapper.Execute().Result;
    DataTable dt = new DataTable("Result");
    for(short i = 0; i < raw.ColumnCount; i++)
        dt.Columns.Add( raw.GetColumnName(i), raw.GetColumnType(i));
    object[] values = new object[raw.ColumnCount];
    foreach (Row row in raw.Rows) {
        for (short i = 0; i < raw.ColumnCount; i++)
            values[i] = row.GetValue(i);
        dt.Rows.Add(values);
    }

Fill a (System.Windows.Forms.)DataGridView

    view.Columns.Clear();
    view.DataSource = null;
    for (short i = 0; i &lt; raw.ColumnCount; i++)
        view.Columns.Add(raw.GetColumnName(i), raw.GetColumnName(i));
    foreach (Row row in raw.Rows) {
        int n = view.Rows.Add();
        for (short i = 0; i < row.ColumnCount; i++) {
            view.Rows[n].Cells[i].Value = row.GetValue(i);
            view.Rows[n].Cells[i].ValueType = row.GetColumnType(i);
        }
    } 


