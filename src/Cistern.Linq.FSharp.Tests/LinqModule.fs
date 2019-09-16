// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.FSharp_Core.Microsoft_FSharp_Collections

open System
open NUnit.Framework

open FSharp.Core.UnitTests.LibraryTestFx

open Cistern.Linq.FSharp

// Various tests for the:
// Microsoft.FSharp.Collections.seq type

(*
[Test Strategy]
Make sure each method works on:
* Integer Seq (value type)
* String Seq  (reference type)
* Empty Seq   (0 elements)
* Null Seq    (null)
*)

[<TestFixture>][<Category "Collections.Seq">][<Category "FSharp.Core.Collections">]
type SeqModule() =

    [<Test>]
    member this.AllPairs() =

        // integer Seq
        let resultInt = Linq.allPairs (seq [1..7]) (seq [11..17])
        let expectedInt =
            seq { for i in 1..7 do
                    for j in 11..17 do
                        yield i, j }
        VerifySeqsEqual expectedInt resultInt

        // string Seq
        let resultStr = Linq.allPairs (seq ["str3";"str4"]) (seq ["str1";"str2"])
        let expectedStr = seq ["str3","str1";"str3","str2";"str4","str1";"str4","str2"]
        VerifySeqsEqual expectedStr resultStr

        // empty Seq
        VerifySeqsEqual Linq.empty <| Linq.allPairs Linq.empty Linq.empty
        VerifySeqsEqual Linq.empty <| Linq.allPairs { 1..7 } Linq.empty
        VerifySeqsEqual Linq.empty <| Linq.allPairs Linq.empty { 1..7 }

        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.allPairs null null |> ignore)
        CheckThrowsArgumentNullException(fun() -> Linq.allPairs null (seq [1..7]) |> ignore)
        CheckThrowsArgumentNullException(fun() -> Linq.allPairs (seq [1..7]) null |> ignore)
        ()

    [<Test>]
    member this.CachedSeq_Clear() =
        
        let evaluatedItems : int list ref = ref []
        let cachedSeq = 
            Linq.initInfinite (fun i -> evaluatedItems := i :: !evaluatedItems; i)
            |> Linq.cache
        
        // Verify no items have been evaluated from the Seq yet
        Assert.AreEqual(List.length !evaluatedItems, 0)
        
        // Force evaluation of 10 elements
        Linq.take 10 cachedSeq
        |> Linq.toList
        |> ignore
        
        // verify ref clear switch length
        Assert.AreEqual(List.length !evaluatedItems, 10)

        // Force evaluation of 10 elements
        Linq.take 10 cachedSeq
        |> Linq.toList
        |> ignore
        
        // Verify ref clear switch length (should be cached)
        Assert.AreEqual(List.length !evaluatedItems, 10)

        
        // Clear
        (box cachedSeq :?> System.IDisposable) .Dispose()
        
        // Force evaluation of 10 elements
        Linq.take 10 cachedSeq
        |> Linq.toList
        |> ignore
        
        // Verify length of evaluatedItemList is 20
        Assert.AreEqual(List.length !evaluatedItems, 20)
        ()
        
    [<Test>]
    member this.Append() =

        // empty Seq 
        let emptySeq1 = Linq.empty
        let emptySeq2 = Linq.empty
        let appendEmptySeq = Linq.append emptySeq1 emptySeq2
        let expectResultEmpty = Linq.empty
           
        VerifySeqsEqual expectResultEmpty appendEmptySeq
          
        // Integer Seq  
        let integerSeq1:seq<int> = seq [0..4]
        let integerSeq2:seq<int> = seq [5..9]
         
        let appendIntergerSeq = Linq.append integerSeq1 integerSeq2
       
        let expectResultInteger = seq { for i in 0..9 -> i}
        
        VerifySeqsEqual expectResultInteger appendIntergerSeq
        
        
        // String Seq
        let stringSeq1:seq<string> = seq ["1";"2"]
        let stringSeq2:seq<string> = seq ["3";"4"]
        
        let appendStringSeq = Linq.append stringSeq1 stringSeq2
        
        let expectedResultString = seq ["1";"2";"3";"4"]
        
        VerifySeqsEqual expectedResultString appendStringSeq
        
        // null Seq
        let nullSeq1 = seq [null;null]

        let nullSeq2 =seq [null;null]

        let appendNullSeq = Linq.append nullSeq1 nullSeq2
        
        let expectedResultNull = seq [ null;null;null;null]
        
        VerifySeqsEqual expectedResultNull appendNullSeq
         
        ()

    [<Test>]
    member this.replicate() =
        // replicate should create multiple copies of the given value
        Assert.IsTrue(Linq.isEmpty <| Linq.replicate 0 null)
        Assert.IsTrue(Linq.isEmpty <| Linq.replicate 0 1)
        Assert.AreEqual(null, Linq.head <| Linq.replicate 1 null)
        Assert.AreEqual(["1";"1"],Linq.replicate 2 "1" |> Linq.toList)

        CheckThrowsArgumentException (fun () ->  Linq.replicate -1 null |> ignore)
        
        
    [<Test>]
    member this.Average() =
        // empty Seq 
        let emptySeq:seq<double> = Linq.empty<double>
        
        CheckThrowsArgumentException (fun () ->  Linq.average emptySeq |> ignore)
        
            
        // double Seq
        let doubleSeq:seq<double> = seq [1.0;2.2;2.5;4.3]
        
        let averageDouble = Linq.average doubleSeq
        
        Assert.IsFalse( averageDouble <> 2.5)
        
        // float32 Seq
        let floatSeq:seq<float32> = seq [ 2.0f;4.4f;5.0f;8.6f]
        
        let averageFloat = Linq.average floatSeq
        
        Assert.IsFalse( averageFloat <> 5.0f)
        
        // decimal Seq
        let decimalSeq:seq<decimal> = seq [ 0M;19M;19.03M]
        
        let averageDecimal = Linq.average decimalSeq
        
        Assert.IsFalse( averageDecimal <> 12.676666666666666666666666667M )
        
        // null Seq
        let nullSeq:seq<double> = null
            
        CheckThrowsArgumentNullException (fun () -> Linq.average nullSeq |> ignore) 
        ()
        
        
    [<Test>]
    member this.AverageBy() =
        // empty Seq 
        let emptySeq:seq<double> = Linq.empty<double>
        
        CheckThrowsArgumentException (fun () ->  Linq.averageBy (fun x -> x+1.0) emptySeq |> ignore)
        
        // double Seq
        let doubleSeq:seq<double> = seq [1.0;2.2;2.5;4.3]
        
        let averageDouble = Linq.averageBy (fun x -> x-2.0) doubleSeq
        
        Assert.IsFalse( averageDouble <> 0.5 )
        
        // float32 Seq
        let floatSeq:seq<float32> = seq [ 2.0f;4.4f;5.0f;8.6f]
        
        let averageFloat = Linq.averageBy (fun x -> x*3.3f)  floatSeq
        
        Assert.IsFalse( averageFloat <> 16.5f )
        
        // decimal Seq
        let decimalSeq:seq<decimal> = seq [ 0M;19M;19.03M]
        
        let averageDecimal = Linq.averageBy (fun x -> x/10.7M) decimalSeq
        
        Assert.IsFalse( averageDecimal <> 1.1847352024922118380062305296M )
        
        // null Seq
        let nullSeq:seq<double> = null
            
        CheckThrowsArgumentNullException (fun () -> Linq.averageBy (fun (x:double)->x+4.0) nullSeq |> ignore) 
        ()
        
    [<Test>]
    member this.Cache() =
        // empty Seq 
        let emptySeq:seq<double> = Linq.empty<double>
        
        let cacheEmpty = Linq.cache emptySeq
        
        let expectedResultEmpty = Linq.empty
        
        VerifySeqsEqual expectedResultEmpty cacheEmpty
               
        // double Seq
        let doubleSeq:seq<double> = seq [1.0;2.2;2.5;4.3]
        
        let cacheDouble = Linq.cache doubleSeq
        
        VerifySeqsEqual doubleSeq cacheDouble
        
            
        // float32 Seq
        let floatSeq:seq<float32> = seq [ 2.0f;4.4f;5.0f;8.6f]
        
        let cacheFloat = Linq.cache floatSeq
        
        VerifySeqsEqual floatSeq cacheFloat
        
        // decimal Seq
        let decimalSeq:seq<decimal> = seq [ 0M; 19M; 19.03M]
        
        let cacheDecimal = Linq.cache decimalSeq
        
        VerifySeqsEqual decimalSeq cacheDecimal
        
        // null Seq
        let nullSeq = seq [null]
        
        let cacheNull = Linq.cache nullSeq
        
        VerifySeqsEqual nullSeq cacheNull
        ()

    [<Test>]
    member this.Case() =

        // integer Seq
        let integerArray = [|1;2|]
        let integerSeq = Linq.cast integerArray
        
        let expectedIntegerSeq = seq [1;2]
        
        VerifySeqsEqual expectedIntegerSeq integerSeq
        
        // string Seq
        let stringArray = [|"a";"b"|]
        let stringSeq = Linq.cast stringArray
        
        let expectedStringSeq = seq["a";"b"]
        
        VerifySeqsEqual expectedStringSeq stringSeq
        
        // empty Seq
        let emptySeq = Linq.cast Linq.empty
        let expectedEmptySeq = Linq.empty
        
        VerifySeqsEqual expectedEmptySeq Linq.empty
        
        // null Seq
        let nullArray = [|null;null|]
        let NullSeq = Linq.cast nullArray
        let expectedNullSeq = seq [null;null]
        
        VerifySeqsEqual expectedNullSeq NullSeq

        CheckThrowsExn<System.InvalidCastException>(fun () -> 
            let strings = 
                integerArray 
                |> Linq.cast<string>               
            for o in strings do ()) 
        
        CheckThrowsExn<System.InvalidCastException>(fun () -> 
            let strings = 
                integerArray 
                |> Linq.cast<string>
                :> System.Collections.IEnumerable // without this upcast the for loop throws, so it should with this upcast too
            for o in strings do ()) 
        
        ()
        
    [<Test>]
    member this.Choose() =
        
        // int Seq
        let intSeq = seq [1..20]    
        let funcInt x = if (x%5=0) then Some x else None       
        let intChoosed = Linq.choose funcInt intSeq
        let expectedIntChoosed = seq { for i = 1 to 4 do yield i*5}
        
        
       
        VerifySeqsEqual expectedIntChoosed intChoosed
        
        // string Seq
        let stringSrc = seq ["list";"List"]
        let funcString x = match x with
                           | "list"-> Some x
                           | "List" -> Some x
                           | _ -> None
        let strChoosed = Linq.choose funcString stringSrc   
        let expectedStrChoose = seq ["list";"List"]
      
        VerifySeqsEqual expectedStrChoose strChoosed
        
        // empty Seq
        let emptySeq = Linq.empty
        let emptyChoosed = Linq.choose funcInt emptySeq
        
        let expectedEmptyChoose = Linq.empty
        
        VerifySeqsEqual expectedEmptyChoose emptySeq
        

        // null Seq
        let nullSeq:seq<'a> = null    
        
        CheckThrowsArgumentNullException (fun () -> Linq.choose funcInt nullSeq |> ignore) 
        ()

    [<Test>]
    member this.ChunkBySize() =

        let verify expected actual =
            Linq.zip expected actual
            |> Linq.iter ((<||) VerifySeqsEqual)

        // int Seq
        verify [[1..4];[5..8]] <| Linq.chunkBySize 4 {1..8}
        verify [[1..4];[5..8];[9..10]] <| Linq.chunkBySize 4 {1..10}
        verify [[1]; [2]; [3]; [4]] <| Linq.chunkBySize 1 {1..4}

        Linq.chunkBySize 2 (Linq.initInfinite id)
        |> Linq.take 3
        |> verify [[0;1];[2;3];[4;5]]

        Linq.chunkBySize 1 (Linq.initInfinite id)
        |> Linq.take 5
        |> verify [[0];[1];[2];[3];[4]]

        // string Seq
        verify [["a"; "b"];["c";"d"];["e"]] <| Linq.chunkBySize 2 ["a";"b";"c";"d";"e"]

        // empty Seq
        verify Linq.empty <| Linq.chunkBySize 3 Linq.empty

        // null Seq
        let nullSeq:seq<_> = null
        CheckThrowsArgumentNullException (fun () -> Linq.chunkBySize 3 nullSeq |> ignore)

        // invalidArg
        CheckThrowsArgumentException (fun () -> Linq.chunkBySize 0 {1..10} |> ignore)
        CheckThrowsArgumentException (fun () -> Linq.chunkBySize -1 {1..10} |> ignore)

        ()

    [<Test>]
    member this.SplitInto() =

        let verify expected actual =
            Linq.zip expected actual
            |> Linq.iter ((<||) VerifySeqsEqual)

        // int Seq
        Linq.splitInto 3 {1..10} |> verify (seq [ {1..4}; {5..7}; {8..10} ])
        Linq.splitInto 3 {1..11} |> verify (seq [ {1..4}; {5..8}; {9..11} ])
        Linq.splitInto 3 {1..12} |> verify (seq [ {1..4}; {5..8}; {9..12} ])

        Linq.splitInto 4 {1..5} |> verify (seq [ [1..2]; [3]; [4]; [5] ])
        Linq.splitInto 20 {1..4} |> verify (seq [ [1]; [2]; [3]; [4] ])

        // string Seq
        Linq.splitInto 3 ["a";"b";"c";"d";"e"] |> verify ([ ["a"; "b"]; ["c";"d"]; ["e"] ])

        // empty Seq
        VerifySeqsEqual [] <| Linq.splitInto 3 []

        // null Seq
        let nullSeq:seq<_> = null
        CheckThrowsArgumentNullException (fun () -> Linq.splitInto 3 nullSeq |> ignore)

        // invalidArg
        CheckThrowsArgumentException (fun () -> Linq.splitInto 0 [1..10] |> ignore)
        CheckThrowsArgumentException (fun () -> Linq.splitInto -1 [1..10] |> ignore)

        ()

    [<Test>]
    member this.Compare() =
    
        // int Seq
        let intSeq1 = seq [1;3;7;9]    
        let intSeq2 = seq [2;4;6;8] 
        let funcInt x y = if (x>y) then x else 0
        let intcompared = Linq.compareWith funcInt intSeq1 intSeq2
       
        Assert.IsFalse( intcompared <> 7 )
        
        // string Seq
        let stringSeq1 = seq ["a"; "b"]
        let stringSeq2 = seq ["c"; "d"]
        let funcString x y = match (x,y) with
                             | "a", "c" -> 0
                             | "b", "d" -> 1
                             |_         -> -1
        let strcompared = Linq.compareWith funcString stringSeq1 stringSeq2  
        Assert.IsFalse( strcompared <> 1 )
         
        // empty Seq
        let emptySeq = Linq.empty
        let emptycompared = Linq.compareWith funcInt emptySeq emptySeq
        
        Assert.IsFalse( emptycompared <> 0 )
       
        // null Seq
        let nullSeq:seq<int> = null    
         
        CheckThrowsArgumentNullException (fun () -> Linq.compareWith funcInt nullSeq emptySeq |> ignore)  
        CheckThrowsArgumentNullException (fun () -> Linq.compareWith funcInt emptySeq nullSeq |> ignore)  
        CheckThrowsArgumentNullException (fun () -> Linq.compareWith funcInt nullSeq nullSeq |> ignore)  

        ()
        
    [<Test>]
    member this.Concat() =
         // integer Seq
        let seqInt = 
            seq { for i in 0..9 do                
                    yield seq {for j in 0..9 do
                                yield i*10+j}}
        let conIntSeq = Seq.concat seqInt
        let expectedIntSeq = seq { for i in 0..99 do yield i}
        
        VerifySeqsEqual expectedIntSeq conIntSeq
         
        // string Seq
        let strSeq = 
            seq { for a in 'a' .. 'b' do
                    for b in 'a' .. 'b' do
                        yield seq [a; b] }
     
        let conStrSeq = Linq.concat strSeq
        let expectedStrSeq = seq ['a';'a';'a';'b';'b';'a';'b';'b';]
        VerifySeqsEqual expectedStrSeq conStrSeq
        
        // Empty Seq
        let emptySeqs = seq [seq[ Linq.empty;Linq.empty];seq[ Linq.empty;Linq.empty]]
        let conEmptySeq = Linq.concat emptySeqs
        let expectedEmptySeq =seq { for i in 1..4 do yield Linq.empty}
        
        VerifySeqsEqual expectedEmptySeq conEmptySeq   

        // null Seq
        let nullSeq:seq<'a> = null
        
        CheckThrowsArgumentNullException (fun () -> Linq.concat nullSeq  |> ignore) 
 
        () 
        
    [<Test>]
    member this.CountBy() =
        // integer Seq
        let funcIntCount_by (x:int) = x%3 
        let seqInt = 
            seq { for i in 0..9 do                
                    yield i}
        let countIntSeq = Linq.countBy funcIntCount_by seqInt
         
        let expectedIntSeq = seq [0,4;1,3;2,3]
        
        VerifySeqsEqual expectedIntSeq countIntSeq
         
        // string Seq
        let funcStrCount_by (s:string) = s.IndexOf("key")
        let strSeq = seq [ "key";"blank key";"key";"blank blank key"]
       
        let countStrSeq = Linq.countBy funcStrCount_by strSeq
        let expectedStrSeq = seq [0,2;6,1;12,1]
        VerifySeqsEqual expectedStrSeq countStrSeq
        
        // Empty Seq
        let emptySeq = Linq.empty
        let countEmptySeq = Linq.countBy funcIntCount_by emptySeq
        let expectedEmptySeq =seq []
        
        VerifySeqsEqual expectedEmptySeq countEmptySeq  

        // null Seq
        let nullSeq:seq<'a> = null
       
        CheckThrowsArgumentNullException (fun () -> Linq.countBy funcIntCount_by nullSeq  |> ignore) 
        () 
    
    [<Test>]
    member this.Distinct() =
        
        // integer Seq
        let IntDistinctSeq =  
            seq { for i in 0..9 do                
                    yield i % 3 }
       
        let DistinctIntSeq = Linq.distinct IntDistinctSeq
       
        let expectedIntSeq = seq [0;1;2]
        
        VerifySeqsEqual expectedIntSeq DistinctIntSeq
     
        // string Seq
        let strDistinctSeq = seq ["elementDup"; "ele1"; "ele2"; "elementDup"]
       
        let DistnctStrSeq = Linq.distinct strDistinctSeq
        let expectedStrSeq = seq ["elementDup"; "ele1"; "ele2"]
        VerifySeqsEqual expectedStrSeq DistnctStrSeq
        
        // Empty Seq
        let emptySeq : seq<decimal * unit>         = Linq.empty
        let distinctEmptySeq : seq<decimal * unit> = Linq.distinct emptySeq
        let expectedEmptySeq : seq<decimal * unit> = seq []
       
        VerifySeqsEqual expectedEmptySeq distinctEmptySeq

        // null Seq
        let nullSeq:seq<unit> = null
       
        CheckThrowsArgumentNullException(fun () -> Linq.distinct nullSeq  |> ignore) 
        () 
    
    [<Test>]
    member this.DistinctBy () =
        // integer Seq
        let funcInt x = x % 3 
        let IntDistinct_bySeq =  
            seq { for i in 0..9 do                
                    yield i }
       
        let distinct_byIntSeq = Linq.distinctBy funcInt IntDistinct_bySeq
        
        let expectedIntSeq = seq [0;1;2]
        
        VerifySeqsEqual expectedIntSeq distinct_byIntSeq
             
        // string Seq
        let funcStrDistinct (s:string) = s.IndexOf("key")
        let strSeq = seq [ "key"; "blank key"; "key dup"; "blank key dup"]
       
        let DistnctStrSeq = Linq.distinctBy funcStrDistinct strSeq
        let expectedStrSeq = seq ["key"; "blank key"]
        VerifySeqsEqual expectedStrSeq DistnctStrSeq
        
        // Empty Seq
        let emptySeq            : seq<int> = Linq.empty
        let distinct_byEmptySeq : seq<int> = Linq.distinctBy funcInt emptySeq
        let expectedEmptySeq    : seq<int> = seq []
       
        VerifySeqsEqual expectedEmptySeq distinct_byEmptySeq

        // null Seq
        let nullSeq : seq<'a> = null
       
        CheckThrowsArgumentNullException(fun () -> Linq.distinctBy funcInt nullSeq  |> ignore) 
        () 

    [<Test>]
    member this.Except() =
        // integer Seq
        let intSeq1 = seq { yield! {1..100}
                            yield! {1..100} }
        let intSeq2 = {1..10}
        let expectedIntSeq = {11..100}

        VerifySeqsEqual expectedIntSeq <| Linq.except intSeq2 intSeq1

        // string Seq
        let strSeq1 = seq ["a"; "b"; "c"; "d"; "a"]
        let strSeq2 = seq ["b"; "c"]
        let expectedStrSeq = seq ["a"; "d"]

        VerifySeqsEqual expectedStrSeq <| Linq.except strSeq2 strSeq1

        // double Seq
        // Sequences with nan do not behave, due to the F# generic equality comparisons
//        let floatSeq1 = seq [1.0; 1.0; System.Double.MaxValue; nan; nan]
//
//        VerifySeqsEqual [1.0; System.Double.MaxValue; nan; nan] <| Linq.except [] floatSeq1
//        VerifySeqsEqual [1.0; System.Double.MaxValue] <| Linq.except [nan] floatSeq1

        // empty Seq
        let emptyIntSeq = Linq.empty<int>
        VerifySeqsEqual {1..100} <| Linq.except emptyIntSeq intSeq1
        VerifySeqsEqual emptyIntSeq <| Linq.except intSeq1 emptyIntSeq
        VerifySeqsEqual emptyIntSeq <| Linq.except emptyIntSeq emptyIntSeq
        VerifySeqsEqual emptyIntSeq <| Linq.except intSeq1 intSeq1

        // null Seq
        let nullSeq : seq<int> = null
        CheckThrowsArgumentNullException(fun () -> Linq.except nullSeq emptyIntSeq |> ignore)
        CheckThrowsArgumentNullException(fun () -> Linq.except emptyIntSeq nullSeq |> ignore)
        CheckThrowsArgumentNullException(fun () -> Linq.except nullSeq nullSeq |> ignore)

        ()

    [<Test>]
    member this.Exists() =

        // Integer Seq
        let funcInt x = (x % 2 = 0) 
        let IntexistsSeq =  
            seq { for i in 0..9 do                
                    yield i}
       
        let ifExistInt = Linq.exists funcInt IntexistsSeq
        
        Assert.IsTrue( ifExistInt) 
            
        // String Seq
        let funcStr (s:string) = s.Contains("key")
        let strSeq = seq ["key"; "blank key"]
       
        let ifExistStr = Linq.exists funcStr strSeq
        
        Assert.IsTrue( ifExistStr)
        
        // Empty Seq
        let emptySeq = Linq.empty
        let ifExistsEmpty = Linq.exists funcInt emptySeq
        
        Assert.IsFalse( ifExistsEmpty)
       
        

        // null Seq
        let nullSeq:seq<'a> = null
           
        CheckThrowsArgumentNullException (fun () -> Linq.exists funcInt nullSeq |> ignore) 
        () 
    
    [<Test>]
    member this.Exists2() =
        // Integer Seq
        let funcInt x y = (x+y)%3=0 
        let Intexists2Seq1 =  seq [1;3;7]
        let Intexists2Seq2 = seq [1;6;3]
            
        let ifExist2Int = Linq.exists2 funcInt Intexists2Seq1 Intexists2Seq2
        Assert.IsTrue( ifExist2Int)
             
        // String Seq
        let funcStr s1 s2 = ((s1 + s2) = "CombinedString")
        let strSeq1 = seq [ "Combined"; "Not Combined"]
        let strSeq2 = seq ["String";    "Other String"]
        let ifexists2Str = Linq.exists2 funcStr strSeq1 strSeq2
        Assert.IsTrue(ifexists2Str)
        
        // Empty Seq
        let emptySeq = Linq.empty
        let ifexists2Empty = Linq.exists2 funcInt emptySeq emptySeq
        Assert.IsFalse( ifexists2Empty)
       
        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.exists2 funcInt nullSeq nullSeq |> ignore) 
        () 
    
    
    [<Test>]
    member this.Filter() =
        // integer Seq
        let funcInt x = if (x % 5 = 0) then true else false
        let IntSeq =
            seq { for i in 1..20 do
                    yield i }
                    
        let filterIntSeq = Linq.filter funcInt IntSeq
          
        let expectedfilterInt = seq [ 5;10;15;20]
        
        VerifySeqsEqual expectedfilterInt filterIntSeq
        
        // string Seq
        let funcStr (s:string) = s.Contains("Expected Content")
        let strSeq = seq [ "Expected Content"; "Not Expected"; "Expected Content"; "Not Expected"]
        
        let filterStrSeq = Linq.filter funcStr strSeq
        
        let expectedfilterStr = seq ["Expected Content"; "Expected Content"]
        
        VerifySeqsEqual expectedfilterStr filterStrSeq    
        // Empty Seq
        let emptySeq = Linq.empty
        let filterEmptySeq = Linq.filter funcInt emptySeq
        
        let expectedEmptySeq =seq []
       
        VerifySeqsEqual expectedEmptySeq filterEmptySeq
       
        

        // null Seq
        let nullSeq:seq<'a> = null
        
        CheckThrowsArgumentNullException (fun () -> Linq.filter funcInt nullSeq  |> ignore) 
        () 
    
    [<Test>]
    member this.Find() =
        
        // integer Seq
        let funcInt x = if (x % 5 = 0) then true else false
        let IntSeq =
            seq { for i in 1..20 do
                    yield i }
                    
        let findInt = Linq.find funcInt IntSeq
        Assert.AreEqual(findInt, 5)  
             
        // string Seq
        let funcStr (s:string) = s.Contains("Expected Content")
        let strSeq = seq [ "Expected Content";"Not Expected"]
        
        let findStr = Linq.find funcStr strSeq
        Assert.AreEqual(findStr, "Expected Content")
        
        // Empty Seq
        let emptySeq = Linq.empty
        
        CheckThrowsKeyNotFoundException(fun () -> Linq.find funcInt emptySeq |> ignore)
       
        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.find funcInt nullSeq |> ignore) 
        ()
    
    [<Test>]
    member this.FindBack() =
        // integer Seq
        let funcInt x = x % 5 = 0
        Assert.AreEqual(20, Linq.findBack funcInt <| seq { 1..20 })
        Assert.AreEqual(15, Linq.findBack funcInt <| seq { 1..19 })
        Assert.AreEqual(5, Linq.findBack funcInt <| seq { 5..9 })

        // string Seq
        let funcStr (s:string) = s.Contains("Expected")
        let strSeq = seq [ "Not Expected"; "Expected Content"]
        let findStr = Linq.findBack funcStr strSeq
        Assert.AreEqual("Expected Content", findStr)

        // Empty Seq
        let emptySeq = Linq.empty
        CheckThrowsKeyNotFoundException(fun () -> Linq.findBack funcInt emptySeq |> ignore)

        // Not found
        let emptySeq = Linq.empty
        CheckThrowsKeyNotFoundException(fun () -> seq { 1..20 } |> Linq.findBack (fun _ -> false) |> ignore)

        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.findBack funcInt nullSeq |> ignore)
        ()

    [<Test>]
    member this.FindIndex() =
        
        // integer Seq
        let digits = [1 .. 100] |> Linq.ofList
        let idx = digits |> Linq.findIndex (fun i -> i.ToString().Length > 1)
        Assert.AreEqual(idx, 9)

        // empty Seq 
        CheckThrowsKeyNotFoundException(fun () -> Linq.findIndex (fun i -> true) Linq.empty |> ignore)
         
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.findIndex (fun i -> true) null |> ignore)
        ()
    
    [<Test>]
    member this.Permute() =
        let mapIndex i = (i + 1) % 4

        // integer seq
        let intSeq = seq { 1..4 }
        let resultInt = Linq.permute mapIndex intSeq
        VerifySeqsEqual (seq [4;1;2;3]) resultInt

        // string seq
        let resultStr = Linq.permute mapIndex [|"Lists"; "are";  "commonly"; "list" |]
        VerifySeqsEqual (seq ["list"; "Lists"; "are";  "commonly" ]) resultStr

        // empty seq
        let resultEpt = Linq.permute mapIndex [||]
        VerifySeqsEqual Linq.empty resultEpt

        // null seq
        let nullSeq = null:string[]
        CheckThrowsArgumentNullException (fun () -> Linq.permute mapIndex nullSeq |> ignore)

        // argument exceptions
        CheckThrowsArgumentException (fun () -> Linq.permute (fun _ -> 10) [0..9] |> Linq.iter ignore)
        CheckThrowsArgumentException (fun () -> Linq.permute (fun _ -> 0) [0..9] |> Linq.iter ignore)
        ()

    [<Test>]
    member this.FindIndexBack() =
        // integer Seq
        let digits = seq { 1..100 }
        let idx = digits |> Linq.findIndexBack (fun i -> i.ToString().Length = 1)
        Assert.AreEqual(idx, 8)

        // string Seq
        let funcStr (s:string) = s.Contains("Expected")
        let strSeq = seq [ "Not Expected"; "Expected Content" ]
        let findStr = Linq.findIndexBack funcStr strSeq
        Assert.AreEqual(1, findStr)

        // empty Seq
        CheckThrowsKeyNotFoundException(fun () -> Linq.findIndexBack (fun i -> true) Linq.empty |> ignore)

        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.findIndexBack (fun i -> true) null |> ignore)
        ()

    [<Test>]
    member this.Pick() =
    
        let digits = [| 1 .. 10 |] |> Linq.ofArray
        let result = Linq.pick (fun i -> if i > 5 then Some(i.ToString()) else None) digits
        Assert.AreEqual(result, "6")
        
        // Empty seq (Bugged, 4173)
        CheckThrowsKeyNotFoundException (fun () -> Linq.pick (fun i -> Some('a')) ([| |] : int[]) |> ignore)

        // Null
        CheckThrowsArgumentNullException (fun () -> Linq.pick (fun i -> Some(i + 0)) null |> ignore)
        ()
        
    [<Test>]
    member this.Fold() =
        let funcInt x y = x+y
             
        let IntSeq =
            seq { for i in 1..10 do
                    yield i}
                    
        let foldInt = Linq.fold funcInt 1 IntSeq
        if foldInt <> 56 then Assert.Fail()
        
        // string Seq
        let funcStr (x:string) (y:string) = x+y
        let strSeq = seq ["B"; "C";  "D" ; "E"]
        let foldStr = Linq.fold  funcStr "A" strSeq
      
        if foldStr <> "ABCDE" then Assert.Fail()
        
        
        // Empty Seq
        let emptySeq = Linq.empty
        let foldEmpty = Linq.fold funcInt 1 emptySeq
        if foldEmpty <> 1 then Assert.Fail()

        // null Seq
        let nullSeq:seq<'a> = null
        
        CheckThrowsArgumentNullException (fun () -> Linq.fold funcInt 1 nullSeq |> ignore) 
        () 



    [<Test>]
    member this.Fold2() =
        Assert.AreEqual([(3,5); (2,3); (1,1)],Linq.fold2 (fun acc x y -> (x,y)::acc) [] (seq [ 1..3 ])  (seq [1..2..6]))

        // integer List  
        let funcInt x y z = x + y + z
        let resultInt = Linq.fold2 funcInt 9 (seq [ 1..10 ]) (seq [1..2..20])
        Assert.AreEqual(164, resultInt)
        
        // string List        
        let funcStr x y z = x + y + z        
        let resultStr = Linq.fold2 funcStr "*" ["a"; "b";  "c" ; "d" ] ["A"; "B";  "C" ; "D" ]        
        Assert.AreEqual("*aAbBcCdD", resultStr)
        
        // empty List
        let emptyArr:int list = [ ]
        let resultEpt = Linq.fold2 funcInt 5 emptyArr emptyArr        
        Assert.AreEqual(5, resultEpt)

        Assert.AreEqual(0,Linq.fold2 funcInt 0 Linq.empty (seq [1]))
        Assert.AreEqual(-1,Linq.fold2 funcInt -1 (seq [1]) Linq.empty)
            
        Assert.AreEqual(2,Linq.fold2 funcInt 0 (seq [1;2]) (seq [1]))
        Assert.AreEqual(4,Linq.fold2 funcInt 0 (seq [1]) (seq [3;6]))

        // null Seq
        let nullSeq:seq<'a> = null     
        
        CheckThrowsArgumentNullException (fun () -> Linq.fold2 funcInt 0 nullSeq (seq [1])  |> ignore) 
        CheckThrowsArgumentNullException (fun () -> Linq.fold2 funcInt 0 (seq [1]) nullSeq |> ignore) 
        ()
        
    [<Test>]
    member this.FoldBack() =
        // int Seq
        let funcInt x y = x-y
        let IntSeq = seq { 1..4 }
        let foldInt = Linq.foldBack funcInt IntSeq 6
        Assert.AreEqual((1-(2-(3-(4-6)))), foldInt)

        // string Seq
        let funcStr (x:string) (y:string) = y.Remove(0,x.Length)
        let strSeq = seq [ "A"; "B"; "C"; "D" ]
        let foldStr = Linq.foldBack  funcStr strSeq "ABCDE"
        Assert.AreEqual("E", foldStr)
        
        // single element
        let funcStr2 elem acc = sprintf "%s%s" elem acc
        let strSeq2 = seq [ "A" ]
        let foldStr2 = Linq.foldBack funcStr2 strSeq2 "X"
        Assert.AreEqual("AX", foldStr2)

        // Empty Seq
        let emptySeq = Linq.empty
        let foldEmpty = Linq.foldBack funcInt emptySeq 1
        Assert.AreEqual(1, foldEmpty)

        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.foldBack funcInt nullSeq 1 |> ignore)

        // Validate that foldBack with the cons operator and the empty list returns a copy of the sequence
        let cons x y = x :: y
        let identityFoldr = Linq.foldBack cons IntSeq []
        Assert.AreEqual([1;2;3;4], identityFoldr)

        ()

    [<Test>]
    member this.foldBack2() =
        // int Seq
        let funcInt x y z = x + y + z
        let intSeq = seq { 1..10 }
        let resultInt = Linq.foldBack2 funcInt intSeq (seq { 1..2..20 }) 9
        Assert.AreEqual(164, resultInt)

        // string Seq
        let funcStr = sprintf "%s%s%s"
        let strSeq = seq [ "A"; "B"; "C"; "D" ]
        let resultStr = Linq.foldBack2  funcStr strSeq (seq [ "a"; "b"; "c"; "d"]) "*"
        Assert.AreEqual("AaBbCcDd*", resultStr)

        // single element
        let strSeqSingle = seq [ "X" ]
        Assert.AreEqual("XAZ", Linq.foldBack2 funcStr strSeqSingle strSeq "Z")
        Assert.AreEqual("AXZ", Linq.foldBack2 funcStr strSeq strSeqSingle "Z")
        Assert.AreEqual("XYZ", Linq.foldBack2 funcStr strSeqSingle (seq [ "Y" ]) "Z")

        // empty Seq
        let emptySeq = Linq.empty
        Assert.AreEqual(1, Linq.foldBack2 funcInt emptySeq emptySeq 1)
        Assert.AreEqual(1, Linq.foldBack2 funcInt emptySeq intSeq 1)
        Assert.AreEqual(1, Linq.foldBack2 funcInt intSeq emptySeq 1)

        // infinite Seq
        let infiniteSeq = Linq.initInfinite (fun i -> 2 * i + 1)
        Assert.AreEqual(164, Linq.foldBack2 funcInt intSeq infiniteSeq 9)
        Assert.AreEqual(164, Linq.foldBack2 funcInt infiniteSeq intSeq 9)

        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.foldBack2 funcInt nullSeq intSeq 1 |> ignore)
        CheckThrowsArgumentNullException (fun () -> Linq.foldBack2 funcInt intSeq nullSeq 1 |> ignore)
        CheckThrowsArgumentNullException (fun () -> Linq.foldBack2 funcInt nullSeq nullSeq 1 |> ignore)

        ()

    [<Test>]
    member this.ForAll() =

        let funcInt x  = if x%2 = 0 then true else false
        let IntSeq =
            seq { for i in 1..10 do
                    yield i*2}
        let for_allInt = Linq.forall funcInt  IntSeq
           
        if for_allInt <> true then Assert.Fail()
        
             
        // string Seq
        let funcStr (x:string)  = x.Contains("a")
        let strSeq = seq ["a"; "ab";  "abc" ; "abcd"]
        let for_allStr = Linq.forall  funcStr strSeq
       
        if for_allStr <> true then Assert.Fail()
        
        
        // Empty Seq
        let emptySeq = Linq.empty
        let for_allEmpty = Linq.forall funcInt emptySeq
        
        if for_allEmpty <> true then Assert.Fail()
        
        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.forall funcInt  nullSeq |> ignore) 
        () 
        
    [<Test>]
    member this.ForAll2() =

        let funcInt x y = if (x+y)%2 = 0 then true else false
        let IntSeq =
            seq { for i in 1..10 do
                    yield i}
                    
        let for_all2Int = Linq.forall2 funcInt  IntSeq IntSeq
           
        if for_all2Int <> true then Assert.Fail()
        
        // string Seq
        let funcStr (x:string) (y:string)  = (x+y).Length = 5
        let strSeq1 = seq ["a"; "ab";  "abc" ; "abcd"]
        let strSeq2 = seq ["abcd"; "abc";  "ab" ; "a"]
        let for_all2Str = Linq.forall2  funcStr strSeq1 strSeq2
       
        if for_all2Str <> true then Assert.Fail()
        
        // Empty Seq
        let emptySeq = Linq.empty
        let for_all2Empty = Linq.forall2 funcInt emptySeq emptySeq
        
        if for_all2Empty <> true then Assert.Fail()

        // null Seq
        let nullSeq:seq<'a> = null
        
        CheckThrowsArgumentNullException (fun () -> Linq.forall2 funcInt  nullSeq nullSeq |> ignore) 
        
    [<Test>]
    member this.GroupBy() =
        
        let funcInt x = x%5
             
        let IntSeq =
            seq { for i in 0 .. 9 do
                    yield i }
                    
        let group_byInt = Linq.groupBy funcInt IntSeq |> Linq.map (fun (i, v) -> i, Linq.toList v)
        
        let expectedIntSeq = 
            seq { for i in 0..4 do
                     yield i, [i; i+5] }
                   
        VerifySeqsEqual group_byInt expectedIntSeq
             
        // string Seq
        let funcStr (x:string) = x.Length
        let strSeq = seq ["length7"; "length 8";  "length7" ; "length  9"]
        
        let group_byStr = Linq.groupBy  funcStr strSeq |> Linq.map (fun (i, v) -> i, Linq.toList v)
        let expectedStrSeq = 
            seq {
                yield 7, ["length7"; "length7"]
                yield 8, ["length 8"]
                yield 9, ["length  9"] }
       
        VerifySeqsEqual expectedStrSeq group_byStr
        
        // Empty Seq
        let emptySeq = Linq.empty
        let group_byEmpty = Linq.groupBy funcInt emptySeq
        let expectedEmptySeq = seq []

        VerifySeqsEqual expectedEmptySeq group_byEmpty
        
        // null Seq
        let nullSeq:seq<'a> = null
        let group_byNull = Linq.groupBy funcInt nullSeq
        CheckThrowsArgumentNullException (fun () -> Linq.iter (fun _ -> ()) group_byNull) 
        () 
    
    [<Test>]
    member this.DisposalOfUnstartedEnumerator() =
        let run = ref false
        let f() = seq {                
                try
                    ()
                finally 
                    run := true
              }
  
        f().GetEnumerator().Dispose() 
        Assert.IsFalse(!run)

    [<Test>]
    member this.WeirdLocalNames() =
       
        let f pc = seq {                
                yield pc
                yield (pc+1)
                yield (pc+2)
              }
  
        let l = f 3 |> Linq.toList
        Assert.AreEqual([3;4;5], l)

        let f i = seq {                
                let pc = i*2
                yield pc
                yield (pc+1)
                yield (pc+2)
              }
        let l = f 3 |> Linq.toList
        Assert.AreEqual([6;7;8], l)

    [<Test>]
    member this.Contains() =

        // Integer Seq
        let intSeq = seq { 0..9 }

        let ifContainsInt = Linq.contains 5 intSeq

        Assert.IsTrue(ifContainsInt)

        // String Seq
        let strSeq = seq ["key"; "blank key"]

        let ifContainsStr = Linq.contains "key" strSeq

        Assert.IsTrue(ifContainsStr)

        // Empty Seq
        let emptySeq = Linq.empty
        let ifContainsEmpty = Linq.contains 5 emptySeq

        Assert.IsFalse(ifContainsEmpty)

        // null Seq
        let nullSeq:seq<'a> = null

        CheckThrowsArgumentNullException (fun () -> Linq.contains 5 nullSeq |> ignore)
