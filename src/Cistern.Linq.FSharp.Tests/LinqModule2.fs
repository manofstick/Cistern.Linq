// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.FSharp_Core.Microsoft_FSharp_Collections

open System
open NUnit.Framework

open FSharp.Core.UnitTests.LibraryTestFx

open Cistern.Linq.FSharp
(* CISTERN NOTES:

0: 
As per https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1.current
Current is undefined for calls before MoveNext is called as well as after MoveNext has returned false.
Cistern does not throw an exception, where as Seq does

1:
The FSharp compiler, given the choice of multiple functions for Max/Min/Sum must have a type defined

*)

type SeqWindowedTestInput<'t> =
    {
        InputSeq : seq<'t>
        WindowSize : int
        ExpectedSeq : seq<'t[]>
        Exception : Type option
    }

[<TestFixture>][<Category "Collections.Seq">][<Category "FSharp.Core.Collections">]
type SeqModule2() =

    [<Test>]
    member this.Hd() =
             
        let IntSeq =
            seq { for i in 0 .. 9 do
                    yield i }
                    
        if Linq.head IntSeq <> 0 then Assert.Fail()
                 
        // string Seq
        let strSeq = seq ["first"; "second";  "third"]
        if Linq.head strSeq <> "first" then Assert.Fail()
         
        // Empty Seq
        let emptySeq = Linq.empty
        CheckThrowsArgumentException ( fun() -> Linq.head emptySeq)
      
        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () ->Linq.head nullSeq) 
        () 

    [<Test>]
    member this.TryHead() =
        // int Seq     
        let IntSeq =
            seq { for i in 0 .. 9 -> i }
                    
        let intResult = Linq.tryHead IntSeq

        // string Seq
        let strResult = Linq.tryHead (seq ["first"; "second";  "third"])
        Assert.AreEqual("first", strResult.Value)
         
        // Empty Seq
        let emptyResult = Linq.tryHead Linq.empty
        Assert.AreEqual(None, emptyResult)
      
        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () ->Linq.head nullSeq) 
        () 
        
    [<Test>]
    member this.Tl() =
        // integer seq  
        let resultInt = Linq.tail <| seq { 1..10 }        
        Assert.AreEqual(Array.ofSeq (seq { 2..10 }), Array.ofSeq resultInt)
        
        // string seq
        let resultStr = Linq.tail <| seq { yield "a"; yield "b"; yield "c"; yield "d" }      
        Assert.AreEqual(Array.ofSeq (seq { yield "b";  yield "c" ; yield "d" }), Array.ofSeq resultStr)
        
        // 1-element seq
        let resultStr2 = Linq.tail <| seq { yield "a" }      
        Assert.AreEqual(Array.ofSeq (Linq.empty : seq<string>), Array.ofSeq resultStr2)

        CheckThrowsArgumentNullException(fun () -> Linq.tail null |> ignore)
        CheckThrowsArgumentException(fun () -> Linq.tail Linq.empty |> Linq.iter (fun _ -> failwith "Should not be reached"))
        ()

    [<Test>]
    member this.Last() =
             
        let IntSeq =
            seq { for i in 0 .. 9 do
                    yield i }
                    
        if Linq.last IntSeq <> 9 then Assert.Fail()
                 
        // string Seq
        let strSeq = seq ["first"; "second";  "third"]
        if Linq.last strSeq <> "third" then Assert.Fail()
         
        // Empty Seq
        let emptySeq = Linq.empty
        CheckThrowsArgumentException ( fun() -> Linq.last emptySeq)
      
        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () ->Linq.last nullSeq) 
        () 

    [<Test>]
    member this.TryLast() =
             
        let IntSeq =
            seq { for i in 0 .. 9 -> i }
                    
        let intResult = Linq.tryLast IntSeq
        Assert.AreEqual(9, intResult.Value)
                 
        // string Seq
        let strResult = Linq.tryLast (seq ["first"; "second";  "third"])
        Assert.AreEqual("third", strResult.Value)
         
        // Empty Seq
        let emptyResult = Linq.tryLast Linq.empty
        Assert.IsTrue(emptyResult.IsNone)
      
        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () ->Linq.tryLast nullSeq |> ignore) 
        () 
        
    [<Test>]
    member this.ExactlyOne() =
             
        let IntSeq =
            seq { for i in 7 .. 7 do
                    yield i }
                    
        if Linq.exactlyOne IntSeq <> 7 then Assert.Fail()
                 
        // string Seq
        let strSeq = seq ["second"]
        if Linq.exactlyOne strSeq <> "second" then Assert.Fail()
         
        // Empty Seq
        let emptySeq = Linq.empty
        CheckThrowsArgumentException ( fun() -> Linq.exactlyOne emptySeq)

        // non-singleton Seq
        let nonSingletonSeq = [ 0 .. 1 ]
        CheckThrowsArgumentException ( fun() -> Linq.exactlyOne nonSingletonSeq |> ignore )

        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.exactlyOne nullSeq) 
        ()

    [<Test>]
    member this.TryExactlyOne() =
        let IntSeq =
            seq { for i in 7 .. 7 do
                    yield i }

        Assert.AreEqual(Some 7, Linq.tryExactlyOne IntSeq)

        // string Seq
        let strSeq = seq ["second"]
        Assert.AreEqual(Some "second", Linq.tryExactlyOne strSeq)

        // Empty Seq
        let emptySeq = Linq.empty
        Assert.AreEqual(None, Linq.tryExactlyOne emptySeq)

        // non-singleton Seq
        let nonSingletonSeq = [ 0 .. 1 ]
        Assert.AreEqual(None, Linq.tryExactlyOne nonSingletonSeq)

        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.tryExactlyOne nullSeq |> ignore)
        ()

    [<Test>]
    member this.Init() =

        let funcInt x = x
        let init_finiteInt = Linq.init 9 funcInt
        let expectedIntSeq = seq [ 0..8]
      
        VerifySeqsEqual expectedIntSeq  init_finiteInt
        
             
        // string Seq
        let funcStr x = x.ToString()
        let init_finiteStr = Linq.init 5  funcStr
        let expectedStrSeq = seq ["0";"1";"2";"3";"4"]

        VerifySeqsEqual expectedStrSeq init_finiteStr
        
        // null Seq
        let funcNull x = null
        let init_finiteNull = Linq.init 3 funcNull
        let expectedNullSeq = seq [ null;null;null]
        
        VerifySeqsEqual expectedNullSeq init_finiteNull
        () 
        
    [<Test>]
    member this.InitInfinite() =

        let funcInt x = x
        let init_infiniteInt = Linq.initInfinite funcInt
        let resultint = Linq.find (fun x -> x =100) init_infiniteInt
        
        Assert.AreEqual(100,resultint)
        
             
        // string Seq
        let funcStr x = x.ToString()
        let init_infiniteStr = Linq.initInfinite  funcStr
        let resultstr = Linq.find (fun x -> x = "100") init_infiniteStr
        
        Assert.AreEqual("100",resultstr)
       
       
    [<Test>]
    member this.IsEmpty() =
        
        //seq int
        let seqint = seq [1;2;3]
        let is_emptyInt = Linq.isEmpty seqint
        
        Assert.IsFalse(is_emptyInt)
              
        //seq str
        let seqStr = seq["first";"second"]
        let is_emptyStr = Linq.isEmpty  seqStr

        Assert.IsFalse(is_emptyInt)
        
        //seq empty
        let seqEmpty = Linq.empty
        let is_emptyEmpty = Linq.isEmpty  seqEmpty
        Assert.IsTrue(is_emptyEmpty) 
        
        //seq null
        let seqnull:seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.isEmpty seqnull |> ignore)
        ()
        
    [<Test>]
    member this.Iter() =
        //seq int
        let seqint =  seq [ 1..3]
        let cacheint = ref 0
       
        let funcint x = cacheint := !cacheint + x
        Linq.iter funcint seqint
        Assert.AreEqual(6,!cacheint)
              
        //seq str
        let seqStr = seq ["first";"second"]
        let cachestr =ref ""
        let funcstr x = cachestr := !cachestr+x
        Linq.iter funcstr seqStr
         
        Assert.AreEqual("firstsecond",!cachestr)
        
         // empty array    
        let emptyseq = Linq.empty
        let resultEpt = ref 0
        Linq.iter (fun x -> Assert.Fail()) emptyseq   

        // null seqay
        let nullseq:seq<'a> =  null
        
        CheckThrowsArgumentNullException (fun () -> Linq.iter funcint nullseq |> ignore)  
        ()
        
    [<Test>]
    member this.Iter2() =
    
        //seq int
        let seqint =  seq [ 1..3]
        let cacheint = ref 0
       
        let funcint x y = cacheint := !cacheint + x+y
        Linq.iter2 funcint seqint seqint
        Assert.AreEqual(12,!cacheint)
              
        //seq str
        let seqStr = seq ["first";"second"]
        let cachestr =ref ""
        let funcstr x y = cachestr := !cachestr+x+y
        Linq.iter2 funcstr seqStr seqStr
         
        Assert.AreEqual("firstfirstsecondsecond",!cachestr)
        
         // empty array    
        let emptyseq = Linq.empty
        let resultEpt = ref 0
        Linq.iter2 (fun x y-> Assert.Fail()) emptyseq  emptyseq 

        // null seqay
        let nullseq:seq<'a> =  null
        CheckThrowsArgumentNullException (fun () -> Linq.iter2 funcint nullseq nullseq |> ignore)  
        
        ()
        
    [<Test>]
    member this.Iteri() =
    
        // seq int
        let seqint =  seq [ 1..10]
        let cacheint = ref 0
       
        let funcint x y = cacheint := !cacheint + x+y
        Linq.iteri funcint seqint
        Assert.AreEqual(100,!cacheint)
              
        // seq str
        let seqStr = seq ["first";"second"]
        let cachestr =ref 0
        let funcstr (x:int) (y:string) = cachestr := !cachestr+ x + y.Length
        Linq.iteri funcstr seqStr
         
        Assert.AreEqual(12,!cachestr)
        
         // empty array    
        let emptyseq = Linq.empty
        let resultEpt = ref 0
        Linq.iteri funcint emptyseq
        Assert.AreEqual(0,!resultEpt)

        // null seqay
        let nullseq:seq<'a> =  null
        CheckThrowsArgumentNullException (fun () -> Linq.iteri funcint nullseq |> ignore)  
        ()

    [<Test>]
    member this.Iteri2() =

        //seq int
        let seqint = seq [ 1..3]
        let cacheint = ref 0
       
        let funcint x y z = cacheint := !cacheint + x + y + z
        Linq.iteri2 funcint seqint seqint
        Assert.AreEqual(15,!cacheint)
              
        //seq str
        let seqStr = seq ["first";"second"]
        let cachestr = ref 0
        let funcstr (x:int) (y:string) (z:string) = cachestr := !cachestr + x + y.Length + z.Length
        Linq.iteri2 funcstr seqStr seqStr
         
        Assert.AreEqual(23,!cachestr)
        
        // empty seq
        let emptyseq = Linq.empty
        let resultEpt = ref 0
        Linq.iteri2 (fun x y z -> Assert.Fail()) emptyseq emptyseq 

        // null seq
        let nullseq:seq<'a> =  null
        CheckThrowsArgumentNullException (fun () -> Linq.iteri2 funcint nullseq nullseq |> ignore)  
        
        // len1 <> len2
        let shorterSeq = seq { 1..3 }
        let longerSeq = seq { 2..2..100 }

        let testSeqLengths seq1 seq2 =
            let cache = ref 0
            let f x y z = cache := !cache + x + y + z
            Linq.iteri2 f seq1 seq2
            !cache

        Assert.AreEqual(21, testSeqLengths shorterSeq longerSeq)
        Assert.AreEqual(21, testSeqLengths longerSeq shorterSeq)

        ()
        
    [<Test>]
    member this.Length() =

         // integer seq  
        let resultInt = Linq.length {1..8}
        if resultInt <> 8 then Assert.Fail()
        
        // string Seq    
        let resultStr = Linq.length (seq ["Lists"; "are";  "commonly" ; "list" ])
        if resultStr <> 4 then Assert.Fail()
        
        // empty Seq     
        let resultEpt = Linq.length Linq.empty
        if resultEpt <> 0 then Assert.Fail()

        // null Seq
        let nullSeq:seq<'a> = null     
        CheckThrowsArgumentNullException (fun () -> Linq.length  nullSeq |> ignore)  
        
        ()
        
    [<Test>]
    member this.Map() =

         // integer Seq
        let funcInt x = 
                match x with
                | _ when x % 2 = 0 -> 10*x            
                | _ -> x
       
        let resultInt = Linq.map funcInt { 1..10 }
        let expectedint = seq [1;20;3;40;5;60;7;80;9;100]
        
        VerifySeqsEqual expectedint resultInt
        
        // string Seq
        let funcStr (x:string) = x.ToLower()
        let resultStr = Linq.map funcStr (seq ["Lists"; "Are";  "Commonly" ; "List" ])
        let expectedSeq = seq ["lists"; "are";  "commonly" ; "list"]
        
        VerifySeqsEqual expectedSeq resultStr
        
        // empty Seq
        let resultEpt = Linq.map funcInt Linq.empty
        VerifySeqsEqual Linq.empty resultEpt

        // null Seq
        let nullSeq:seq<'a> = null 
        CheckThrowsArgumentNullException (fun () -> Linq.map funcStr nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.Map2() =
         // integer Seq
        let funcInt x y = x+y
        let resultInt = Linq.map2 funcInt { 1..10 } {2..2..20} 
        let expectedint = seq [3;6;9;12;15;18;21;24;27;30]
        
        VerifySeqsEqual expectedint resultInt
        
        // string Seq
        let funcStr (x:int) (y:string) = x+y.Length
        let resultStr = Linq.map2 funcStr (seq[3;6;9;11]) (seq ["Lists"; "Are";  "Commonly" ; "List" ])
        let expectedSeq = seq [8;9;17;15]
        
        VerifySeqsEqual expectedSeq resultStr
        
        // empty Seq
        let resultEpt = Linq.map2 funcInt Linq.empty Linq.empty
        VerifySeqsEqual Linq.empty resultEpt

        // null Seq
        let nullSeq:seq<'a> = null 
        let validSeq = seq [1]
        CheckThrowsArgumentNullException (fun () -> Linq.map2 funcInt nullSeq validSeq |> ignore)
        
        ()

    [<Test>]
    member this.Map3() = 
        // Integer seq
        let funcInt a b c = (a + b) * c
        let resultInt = Linq.map3 funcInt { 1..8 } { 2..9 } { 3..10 }
        let expectedInt = seq [9; 20; 35; 54; 77; 104; 135; 170]
        VerifySeqsEqual expectedInt resultInt

        // First seq is shorter
        VerifySeqsEqual (seq [9; 20]) (Linq.map3 funcInt { 1..2 } { 2..9 } { 3..10 })
        // Second seq is shorter
        VerifySeqsEqual (seq [9; 20; 35]) (Linq.map3 funcInt { 1..8 } { 2..4 } { 3..10 })
        // Third seq is shorter
        VerifySeqsEqual (seq [9; 20; 35; 54]) (Linq.map3 funcInt { 1..8 } { 2..6 } { 3..6 })

        // String seq
        let funcStr a b c = a + b + c
        let resultStr = Linq.map3 funcStr ["A";"B";"C";"D"] ["a";"b";"c";"d"] ["1";"2";"3";"4"]
        let expectedStr = seq ["Aa1";"Bb2";"Cc3";"Dd4"]
        VerifySeqsEqual expectedStr resultStr

        // Empty seq
        let resultEmpty = Linq.map3 funcStr Linq.empty Linq.empty Linq.empty
        VerifySeqsEqual Linq.empty resultEmpty

        // Null seq
        let nullSeq = null : seq<_>
        let nonNullSeq = seq [1]
        CheckThrowsArgumentNullException (fun () -> Linq.map3 funcInt nullSeq nonNullSeq nullSeq |> ignore)

        ()

    [<Test>]
    member this.MapFold() =
        // integer Seq
        let funcInt acc x = if x % 2 = 0 then 10*x, acc + 1 else x, acc
        let resultInt,resultIntAcc = Linq.mapFold funcInt 100 <| seq { 1..10 }
        VerifySeqsEqual (seq [ 1;20;3;40;5;60;7;80;9;100 ]) resultInt
        Assert.AreEqual(105, resultIntAcc)

        // string Seq
        let funcStr acc (x:string) = match x.Length with 0 -> "empty", acc | _ -> x.ToLower(), sprintf "%s%s" acc x
        let resultStr,resultStrAcc = Linq.mapFold funcStr "" <| seq [ "";"BB";"C";"" ]
        VerifySeqsEqual (seq [ "empty";"bb";"c";"empty" ]) resultStr
        Assert.AreEqual("BBC", resultStrAcc)

        // empty Seq
        let resultEpt,resultEptAcc = Linq.mapFold funcInt 100 Linq.empty
        VerifySeqsEqual Linq.empty resultEpt
        Assert.AreEqual(100, resultEptAcc)

        // null Seq
        let nullArr = null:seq<string>
        CheckThrowsArgumentNullException (fun () -> Linq.mapFold funcStr "" nullArr |> ignore)

        ()

    [<Test>]
    member this.MapFoldBack() =
        // integer Seq
        let funcInt x acc = if acc < 105 then 10*x, acc + 2 else x, acc
        let resultInt,resultIntAcc = Linq.mapFoldBack funcInt (seq { 1..10 }) 100
        VerifySeqsEqual (seq [ 1;2;3;4;5;6;7;80;90;100 ]) resultInt
        Assert.AreEqual(106, resultIntAcc)

        // string Seq
        let funcStr (x:string) acc = match x.Length with 0 -> "empty", acc | _ -> x.ToLower(), sprintf "%s%s" acc x
        let resultStr,resultStrAcc = Linq.mapFoldBack funcStr (seq [ "";"BB";"C";"" ]) ""
        VerifySeqsEqual (seq [ "empty";"bb";"c";"empty" ]) resultStr
        Assert.AreEqual("CBB", resultStrAcc)

        // empty Seq
        let resultEpt,resultEptAcc = Linq.mapFoldBack funcInt Linq.empty 100
        VerifySeqsEqual Linq.empty resultEpt
        Assert.AreEqual(100, resultEptAcc)

        // null Seq
        let nullArr = null:seq<string>
        CheckThrowsArgumentNullException (fun () -> Linq.mapFoldBack funcStr nullArr "" |> ignore)

        ()

    member private this.MapWithSideEffectsTester (map : (int -> int) -> seq<int> -> seq<int>) expectExceptions =
        let i = ref 0
        let f x = i := !i + 1; x*x
        let e = ([1;2] |> map f).GetEnumerator()
        
        if expectExceptions then
            (*CISTERN[0]:CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)*)
            Assert.AreEqual(0, !i)
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)

        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(2, !i)
        if expectExceptions then
            (*CISTERN[0]:CheckThrowsInvalidOperationExn (fun _ -> e.Current |> ignore)*)
            Assert.AreEqual(2, !i)

        
        i := 0
        let e = ([] |> map f).GetEnumerator()
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        
        
    member private this.MapWithExceptionTester (map : (int -> int) -> seq<int> -> seq<int>) =
        let raiser x = if x > 0 then raise(NotSupportedException()) else x
        let e = (map raiser [0; 1]).GetEnumerator()
        Assert.IsTrue(e.MoveNext()) // should not throw
        Assert.AreEqual(0, e.Current)
        CheckThrowsNotSupportedException(fun _ -> e.MoveNext() |> ignore)
        Assert.AreEqual(0, e.Current) // should not throw

    [<Test>]
    member this.MapWithSideEffects () =
        this.MapWithSideEffectsTester Linq.map true
        
    [<Test>]
    member this.MapWithException () =
        this.MapWithExceptionTester Linq.map

        
    [<Test>]
    member this.SingletonCollectWithSideEffects () =
        this.MapWithSideEffectsTester (fun f-> Linq.collect (f >> Linq.singleton)) true
        
    [<Test>]
    member this.SingletonCollectWithException () =
        this.MapWithExceptionTester (fun f-> Linq.collect (f >> Linq.singleton))

    [<Test>]
    member this.SystemLinqSelectWithSideEffects () =
        this.MapWithSideEffectsTester (fun f s -> System.Linq.Enumerable.Select(s, Func<_,_>(f))) false
        
    [<Test>]
    member this.SystemLinqSelectWithException () =
        this.MapWithExceptionTester (fun f s -> System.Linq.Enumerable.Select(s, Func<_,_>(f)))
        
    [<Test>]
    member this.MapiWithSideEffects () =
        let i = ref 0
        let f _ x = i := !i + 1; x*x
        let e = ([1;2] |> Linq.mapi f).GetEnumerator()
        
        (*CISTERN[0]:CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)*)
        Assert.AreEqual(0, !i)
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)
        
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(2, !i)
        (*CISTERN[0]:CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)*)
        Assert.AreEqual(2, !i)
        
        i := 0
        let e = ([] |> Linq.mapi f).GetEnumerator()
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        
    [<Test>]
    member this.Map2WithSideEffects () =
        let i = ref 0
        let f x y = i := !i + 1; x*x
        let e = (Linq.map2 f [1;2] [1;2]).GetEnumerator()
        
        CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)
        Assert.AreEqual(0, !i)
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)

        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(2,!i)
        CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)
        Assert.AreEqual(2, !i)
        
        i := 0
        let e = (Linq.map2 f [] []).GetEnumerator()
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        
    [<Test>]
    member this.Mapi2WithSideEffects () =
        let i = ref 0
        let f _ x y = i := !i + 1; x*x
        let e = (Linq.mapi2 f [1;2] [1;2]).GetEnumerator()

        CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)
        Assert.AreEqual(0, !i)
        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)
        let _ = e.Current
        Assert.AreEqual(1, !i)

        if not (e.MoveNext()) then Assert.Fail()
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)
        let _ = e.Current
        Assert.AreEqual(2, !i)

        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(2,!i)
        CheckThrowsInvalidOperationExn  (fun _ -> e.Current|>ignore)
        Assert.AreEqual(2, !i)

        i := 0
        let e = (Linq.mapi2 f [] []).GetEnumerator()
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)
        if e.MoveNext() then Assert.Fail()
        Assert.AreEqual(0,!i)

    [<Test>]
    member this.Collect() =
         // integer Seq
        let funcInt x = seq [x+1]
        let resultInt = Linq.collect funcInt { 1..10 } 
       
        let expectedint = seq {2..11}
        
        VerifySeqsEqual expectedint resultInt

        // string Seq
        let funcStr (y:string) = y+"ist"

        let resultStr = Linq.collect funcStr (seq ["L"])

        let expectedSeq = seq ['L';'i';'s';'t']

        VerifySeqsEqual expectedSeq resultStr

        // empty Seq
        let resultEpt = Linq.collect funcInt Linq.empty
        VerifySeqsEqual Linq.empty resultEpt

        // null Seq
        let nullSeq:seq<'a> = null 

        CheckThrowsArgumentNullException (fun () -> Linq.collect funcInt nullSeq |> ignore)

        ()

    [<Test>]
    member this.Mapi() =

         // integer Seq
        let funcInt x y = x+y
        let resultInt = Linq.mapi funcInt { 10..2..20 } 
        let expectedint = seq [10;13;16;19;22;25]
        
        VerifySeqsEqual expectedint resultInt
        
        // string Seq
        let funcStr (x:int) (y:string) =x+y.Length
       
        let resultStr = Linq.mapi funcStr (seq ["Lists"; "Are";  "Commonly" ; "List" ])
        let expectedStr = seq [5;4;10;7]
         
        VerifySeqsEqual expectedStr resultStr
        
        // empty Seq
        let resultEpt = Linq.mapi funcInt Linq.empty
        VerifySeqsEqual Linq.empty resultEpt

        // null Seq
        let nullSeq:seq<'a> = null 
       
        CheckThrowsArgumentNullException (fun () -> Linq.mapi funcInt nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.Mapi2() =
         // integer Seq
        let funcInt x y z = x+y+z
        let resultInt = Linq.mapi2 funcInt { 1..10 } {2..2..20}
        let expectedint = seq [3;7;11;15;19;23;27;31;35;39]

        VerifySeqsEqual expectedint resultInt

        // string Seq
        let funcStr (x:int) (y:int) (z:string) = x+y+z.Length
        let resultStr = Linq.mapi2 funcStr (seq[3;6;9;11]) (seq ["Lists"; "Are";  "Commonly" ; "List" ])
        let expectedSeq = seq [8;10;19;18]

        VerifySeqsEqual expectedSeq resultStr

        // empty Seq
        let resultEpt = Linq.mapi2 funcInt Linq.empty Linq.empty
        VerifySeqsEqual Linq.empty resultEpt

        // null Seq
        let nullSeq:seq<'a> = null
        let validSeq = seq [1]
        CheckThrowsArgumentNullException (fun () -> Linq.mapi2 funcInt nullSeq validSeq |> ignore)

        // len1 <> len2
        let shorterSeq = seq { 1..10 }
        let longerSeq = seq { 2..20 }

        let testSeqLengths seq1 seq2 =
            let f x y z = x + y + z
            Linq.mapi2 f seq1 seq2

        VerifySeqsEqual (seq [3;6;9;12;15;18;21;24;27;30]) (testSeqLengths shorterSeq longerSeq)
        VerifySeqsEqual (seq [3;6;9;12;15;18;21;24;27;30]) (testSeqLengths longerSeq shorterSeq)

    [<Test>]
    member this.Indexed() =

         // integer Seq
        let resultInt = Linq.indexed { 10..2..20 }
        let expectedint = seq [(0,10);(1,12);(2,14);(3,16);(4,18);(5,20)]

        VerifySeqsEqual expectedint resultInt

        // string Seq
        let resultStr = Linq.indexed (seq ["Lists"; "Are"; "Commonly"; "List" ])
        let expectedStr = seq [(0,"Lists");(1,"Are");(2,"Commonly");(3,"List")]

        VerifySeqsEqual expectedStr resultStr

        // empty Seq
        let resultEpt = Linq.indexed Linq.empty
        VerifySeqsEqual Linq.empty resultEpt

        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.indexed nullSeq |> ignore)

        ()

    [<Test>]
    member this.Max() =
         // integer Seq
        let resultInt = Linq.max { 10..20 } 
        
        Assert.AreEqual(20,resultInt)
        
        // string Seq
       
        let resultStr = Linq.max (seq ["Lists"; "Are";  "MaxString" ; "List" ])
        Assert.AreEqual("MaxString",resultStr)
          
        // empty Seq
        CheckThrowsArgumentException(fun () -> Linq.max ( Linq.empty : seq<float>) |> ignore)

        // null Seq
        let nullSeq:seq<(*CISTERN[1]:'a*)int> = null 
        CheckThrowsArgumentNullException (fun () -> Linq.max nullSeq |> ignore)
        ()
        
    [<Test>]
    member this.MaxBy() =
    
        // integer Seq
        let funcInt x = x % 8
        let resultInt = Linq.maxBy funcInt { 2..2..20 } 
        Assert.AreEqual(6,resultInt)
        
        // string Seq
        let funcStr (x:string)  =x.Length 
        let resultStr = Linq.maxBy funcStr (seq ["Lists"; "Are";  "Commonly" ; "List" ])
        Assert.AreEqual("Commonly",resultStr)
          
        // empty Seq
        CheckThrowsArgumentException (fun () -> Linq.maxBy funcInt (Linq.empty : seq<int>) |> ignore)
        
        // null Seq
        let nullSeq:seq<'a> = null 
        CheckThrowsArgumentNullException (fun () ->Linq.maxBy funcInt nullSeq |> ignore)
        
        ()
        
    [<Test>]
    member this.MinBy() =
    
        // integer Seq
        let funcInt x = x % 8
        let resultInt = Linq.minBy funcInt { 2..2..20 } 
        Assert.AreEqual(8,resultInt)
        
        // string Seq
        let funcStr (x:string)  =x.Length 
        let resultStr = Linq.minBy funcStr (seq ["Lists"; "Are";  "Commonly" ; "List" ])
        Assert.AreEqual("Are",resultStr)
          
        // empty Seq
        CheckThrowsArgumentException (fun () -> Linq.minBy funcInt (Linq.empty : seq<int>) |> ignore) 
        
        // null Seq
        let nullSeq:seq<'a> = null 
        CheckThrowsArgumentNullException (fun () ->Linq.minBy funcInt nullSeq |> ignore)
        
        ()
        
          
    [<Test>]
    member this.Min() =

         // integer Seq
        let resultInt = Linq.min { 10..20 } 
        Assert.AreEqual(10,resultInt)
        
        // string Seq
        let resultStr = Linq.min (seq ["Lists"; "Are";  "minString" ; "List" ])
        Assert.AreEqual("Are",resultStr)
          
        // empty Seq
        CheckThrowsArgumentException (fun () -> Linq.min (Linq.empty : seq<int>) |> ignore) 
        
        // null Seq
        let nullSeq:seq<(*CISTERN[1]:'a*)int> = null 
        CheckThrowsArgumentNullException (fun () -> Linq.min nullSeq |> ignore)

        ()

    [<Test>]
    member this.Item() =
         // integer Seq
        let resultInt = Linq.item 3 { 10..20 }
        Assert.AreEqual(13, resultInt)

        // string Seq
        let resultStr = Linq.item 2 (seq ["Lists"; "Are"; "Cool" ; "List" ])
        Assert.AreEqual("Cool", resultStr)

        // empty Seq
        CheckThrowsArgumentException(fun () -> Linq.item 0 (Linq.empty : seq<decimal>) |> ignore)

        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () ->Linq.item 3 nullSeq |> ignore)

        // Negative index
        for i = -1 downto -10 do
           CheckThrowsArgumentException (fun () -> Linq.item i { 10 .. 20 } |> ignore)

        // Out of range
        for i = 11 to 20 do
           CheckThrowsArgumentException (fun () -> Linq.item i { 10 .. 20 } |> ignore)

    [<Test>]
    member this.``item should fail with correct number of missing elements``() =
        try
            Linq.item 0 (Array.zeroCreate<int> 0) |> ignore
            failwith "error expected"
        with
        | exn when exn.Message.Contains("seq was short by 1 element") -> ()

        try
            Linq.item 2 (Array.zeroCreate<int> 0) |> ignore
            failwith "error expected"
        with
        | exn when exn.Message.Contains("seq was short by 3 elements") -> ()

    [<Test>]
    member this.Of_Array() =
         // integer Seq
        let resultInt = Linq.ofArray [|1..10|]
        let expectedInt = {1..10}
         
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
        let resultStr = Linq.ofArray [|"Lists"; "Are";  "ofArrayString" ; "List" |]
        let expectedStr = seq ["Lists"; "Are";  "ofArrayString" ; "List" ]
        VerifySeqsEqual expectedStr resultStr
          
        // empty Seq 
        let resultEpt = Linq.ofArray [| |] 
        VerifySeqsEqual resultEpt Linq.empty
       
        ()
        
    [<Test>]
    member this.Of_List() =
         // integer Seq
        let resultInt = Linq.ofList [1..10]
        let expectedInt = {1..10}
         
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
       
        let resultStr =Linq.ofList ["Lists"; "Are";  "ofListString" ; "List" ]
        let expectedStr = seq ["Lists"; "Are";  "ofListString" ; "List" ]
        VerifySeqsEqual expectedStr resultStr
          
        // empty Seq 
        let resultEpt = Linq.ofList [] 
        VerifySeqsEqual resultEpt Linq.empty
        ()
        
          
    [<Test>]
    member this.Pairwise() =
         // integer Seq
        let resultInt = Linq.pairwise {1..3}
       
        let expectedInt = seq [1,2;2,3]
         
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
        let resultStr =Linq.pairwise ["str1"; "str2";"str3" ]
        let expectedStr = seq ["str1","str2";"str2","str3"]
        VerifySeqsEqual expectedStr resultStr
          
        // empty Seq 
        let resultEpt = Linq.pairwise [] 
        VerifySeqsEqual resultEpt Linq.empty
       
        ()
        
    [<Test>]
    member this.Reduce() =
         
        // integer Seq
        let resultInt = Linq.reduce (fun x y -> x/y) (seq [5*4*3*2; 4;3;2;1])
        Assert.AreEqual(5,resultInt)
        
        // string Seq
        let resultStr = Linq.reduce (fun (x:string) (y:string) -> x.Remove(0,y.Length)) (seq ["ABCDE";"A"; "B";  "C" ; "D" ])
        Assert.AreEqual("E",resultStr) 
       
        // empty Seq 
        CheckThrowsArgumentException (fun () -> Linq.reduce (fun x y -> x/y)  Linq.empty |> ignore)
        
        // null Seq
        let nullSeq : seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.reduce (fun (x:string) (y:string) -> x.Remove(0,y.Length))  nullSeq  |> ignore)   
        ()

    [<Test>]
    member this.ReduceBack() =
        // int Seq
        let funcInt x y = x - y
        let IntSeq = seq { 1..4 }
        let reduceInt = Linq.reduceBack funcInt IntSeq
        Assert.AreEqual((1-(2-(3-4))), reduceInt)

        // string Seq
        let funcStr (x:string) (y:string) = y.Remove(0,x.Length)
        let strSeq = seq [ "A"; "B"; "C"; "D" ; "ABCDE" ]
        let reduceStr = Linq.reduceBack  funcStr strSeq
        Assert.AreEqual("E", reduceStr)
        
        // string Seq
        let funcStr2 elem acc = sprintf "%s%s" elem acc
        let strSeq2 = seq [ "A" ]
        let reduceStr2 = Linq.reduceBack  funcStr2 strSeq2
        Assert.AreEqual("A", reduceStr2)

        // Empty Seq
        CheckThrowsArgumentException (fun () -> Linq.reduceBack funcInt Linq.empty |> ignore)

        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.reduceBack funcInt nullSeq |> ignore)

        ()

    [<Test>]
    member this.Rev() =
        // integer Seq
        let resultInt = Linq.rev (seq [5;4;3;2;1])
        VerifySeqsEqual (seq[1;2;3;4;5]) resultInt

        // string Seq
        let resultStr = Linq.rev (seq ["A"; "B";  "C" ; "D" ])
        VerifySeqsEqual (seq["D";"C";"B";"A"]) resultStr

        // empty Seq
        VerifySeqsEqual Linq.empty (Linq.rev Linq.empty)

        // null Seq
        let nullSeq : seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.rev nullSeq  |> ignore)
        ()

    [<Test>]
    member this.Scan() =
        // integer Seq
        let funcInt x y = x+y
        let resultInt = Linq.scan funcInt 9 {1..10}
        let expectedInt = seq [9;10;12;15;19;24;30;37;45;54;64]
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
        let funcStr x y = x+y
        let resultStr =Linq.scan funcStr "x" ["str1"; "str2";"str3" ]
        
        let expectedStr = seq ["x";"xstr1"; "xstr1str2";"xstr1str2str3"]
        VerifySeqsEqual expectedStr resultStr
          
        // empty Seq 
        let resultEpt = Linq.scan funcInt 5 Linq.empty 
       
        VerifySeqsEqual resultEpt (seq [ 5])
       
        // null Seq
        let seqNull:seq<'a> = null
        CheckThrowsArgumentNullException(fun() -> Linq.scan funcInt 5 seqNull |> ignore)
        ()
        
    [<Test>]
    member this.ScanBack() =
        // integer Seq
        let funcInt x y = x+y
        let resultInt = Linq.scanBack funcInt { 1..10 } 9
        let expectedInt = seq [64;63;61;58;54;49;43;36;28;19;9]
        VerifySeqsEqual expectedInt resultInt

        // string Seq
        let funcStr x y = x+y
        let resultStr = Linq.scanBack funcStr (seq ["A";"B";"C";"D"]) "X"
        let expectedStr = seq ["ABCDX";"BCDX";"CDX";"DX";"X"]
        VerifySeqsEqual expectedStr resultStr

        // empty Seq
        let resultEpt = Linq.scanBack funcInt Linq.empty 5
        let expectedEpt = seq [5]
        VerifySeqsEqual expectedEpt resultEpt

        // null Seq
        let seqNull:seq<'a> = null
        CheckThrowsArgumentNullException(fun() -> Linq.scanBack funcInt seqNull 5 |> ignore)

        // exception cases
        let funcEx x (s:'State) = raise <| new System.FormatException() : 'State
        // calling scanBack with funcEx does not throw
        let resultEx = Linq.scanBack funcEx (seq {1..10}) 0
        // reading from resultEx throws
        CheckThrowsFormatException(fun() -> Linq.head resultEx |> ignore)

        // Result consumes entire input sequence as soon as it is accesses an element
        let i = ref 0
        let funcState x s = (i := !i + x); x+s
        let resultState = Linq.scanBack funcState (seq {1..3}) 0
        Assert.AreEqual(0, !i)
        use e = resultState.GetEnumerator()
        Assert.AreEqual(6, !i)

        ()

    [<Test>]
    member this.Singleton() =
        // integer Seq
        let resultInt = Linq.singleton 1
       
        let expectedInt = seq [1]
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
        let resultStr =Linq.singleton "str1"
        let expectedStr = seq ["str1"]
        VerifySeqsEqual expectedStr resultStr
         
        // null Seq
        let resultNull = Linq.singleton null
        let expectedNull = seq [null]
        VerifySeqsEqual expectedNull resultNull
        ()
    
        
    [<Test>]
    member this.Skip() =
    
        // integer Seq
        let resultInt = Linq.skip 2 (seq [1;2;3;4])
        let expectedInt = seq [3;4]
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
        let resultStr =Linq.skip 2 (seq ["str1";"str2";"str3";"str4"])
        let expectedStr = seq ["str3";"str4"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty Seq 
        let resultEpt = Linq.skip 0 Linq.empty 
        VerifySeqsEqual resultEpt Linq.empty
        
         
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.skip 1 null |> ignore)
        ()
       
    [<Test>]
    member this.Skip_While() =
    
        // integer Seq
        let funcInt x = (x < 3)
        let resultInt = Linq.skipWhile funcInt (seq [1;2;3;4;5;6])
        let expectedInt = seq [3;4;5;6]
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
        let funcStr (x:string) = x.Contains(".")
        let resultStr =Linq.skipWhile funcStr (seq [".";"asdfasdf.asdfasdf";"";"";"";"";"";"";"";"";""])
        let expectedStr = seq ["";"";"";"";"";"";"";"";""]
        VerifySeqsEqual expectedStr resultStr
        
        // empty Seq 
        let resultEpt = Linq.skipWhile funcInt Linq.empty 
        VerifySeqsEqual resultEpt Linq.empty
        
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.skipWhile funcInt null |> ignore)
        ()
       
    [<Test>]
    member this.Sort() =

        // integer Seq
        let resultInt = Linq.sort (seq [1;3;2;4;6;5;7])
        let expectedInt = {1..7}
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
       
        let resultStr =Linq.sort (seq ["str1";"str3";"str2";"str4"])
        let expectedStr = seq ["str1";"str2";"str3";"str4"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty Seq 
        let resultEpt = Linq.sort Linq.empty 
        VerifySeqsEqual resultEpt Linq.empty
         
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.sort null  |> ignore)
        ()
        
    [<Test>]
    member this.SortBy() =

        // integer Seq
        let funcInt x = Math.Abs(x-5)
        let resultInt = Linq.sortBy funcInt (seq [1;2;4;5;7])
        let expectedInt = seq [5;4;7;2;1]
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
        let funcStr (x:string) = x.IndexOf("key")
        let resultStr =Linq.sortBy funcStr (seq ["st(key)r";"str(key)";"s(key)tr";"(key)str"])
        
        let expectedStr = seq ["(key)str";"s(key)tr";"st(key)r";"str(key)"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty Seq 
        let resultEpt = Linq.sortBy funcInt Linq.empty 
        VerifySeqsEqual resultEpt Linq.empty
         
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.sortBy funcInt null  |> ignore)
        ()

    [<Test>]
    member this.SortDescending() =

        // integer Seq
        let resultInt = Linq.sortDescending (seq [1;3;2;Int32.MaxValue;4;6;Int32.MinValue;5;7;0])
        let expectedInt = seq{
            yield Int32.MaxValue;
            yield! seq{ 7..-1..0 }
            yield Int32.MinValue
        }
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
       
        let resultStr = Linq.sortDescending (seq ["str1";null;"str3";"";"Str1";"str2";"str4"])
        let expectedStr = seq ["str4";"str3";"str2";"str1";"Str1";"";null]
        VerifySeqsEqual expectedStr resultStr
        
        // empty Seq 
        let resultEpt = Linq.sortDescending Linq.empty 
        VerifySeqsEqual resultEpt Linq.empty

        // tuple Seq
        let tupSeq = (seq[(2,"a");(1,"d");(1,"b");(1,"a");(2,"x");(2,"b");(1,"x")])
        let resultTup = Linq.sortDescending tupSeq
        let expectedTup = (seq[(2,"x");(2,"b");(2,"a");(1,"x");(1,"d");(1,"b");(1,"a")])   
        VerifySeqsEqual  expectedTup resultTup
         
        // float Seq
        let minFloat,maxFloat,epsilon = System.Double.MinValue,System.Double.MaxValue,System.Double.Epsilon
        let floatSeq = seq [0.0; 0.5; 2.0; 1.5; 1.0; minFloat;maxFloat;epsilon;-epsilon]
        let resultFloat = Linq.sortDescending floatSeq
        let expectedFloat = seq [maxFloat; 2.0; 1.5; 1.0; 0.5; epsilon; 0.0; -epsilon; minFloat; ]
        VerifySeqsEqual expectedFloat resultFloat

        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.sort null  |> ignore)
        ()
        
    [<Test>]
    member this.SortByDescending() =

        // integer Seq
        let funcInt x = Math.Abs(x-5)
        let resultInt = Linq.sortByDescending funcInt (seq [1;2;4;5;7])
        let expectedInt = seq [1;2;7;4;5]
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
        let funcStr (x:string) = x.IndexOf("key")
        let resultStr =Linq.sortByDescending funcStr (seq ["st(key)r";"str(key)";"s(key)tr";"(key)str"])
        
        let expectedStr = seq ["str(key)";"st(key)r";"s(key)tr";"(key)str"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty Seq 
        let resultEpt = Linq.sortByDescending funcInt Linq.empty 
        VerifySeqsEqual resultEpt Linq.empty

        // tuple Seq
        let tupSeq = (seq[(2,"a");(1,"d");(1,"b");(1,"a");(2,"x");(2,"b");(1,"x")])
        let resultTup = Linq.sortByDescending snd tupSeq         
        let expectedTup = (seq[(2,"x");(1,"x");(1,"d");(1,"b");(2,"b");(2,"a");(1,"a")])
        VerifySeqsEqual  expectedTup resultTup
         
        // float Seq
        let minFloat,maxFloat,epsilon = System.Double.MinValue,System.Double.MaxValue,System.Double.Epsilon
        let floatSeq = seq [0.0; 0.5; 2.0; 1.5; 1.0; minFloat;maxFloat;epsilon;-epsilon]
        let resultFloat = Linq.sortByDescending id floatSeq
        let expectedFloat = seq [maxFloat; 2.0; 1.5; 1.0; 0.5; epsilon; 0.0; -epsilon; minFloat; ]
        VerifySeqsEqual expectedFloat resultFloat

        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.sortByDescending funcInt null  |> ignore)
        ()
        
    member this.SortWith() =

        // integer Seq
        let intComparer a b = compare (a%3) (b%3)
        let resultInt = Linq.sortWith intComparer (seq {0..10})
        let expectedInt = seq [0;3;6;9;1;4;7;10;2;5;8]
        VerifySeqsEqual expectedInt resultInt

        // string Seq
        let resultStr = Linq.sortWith compare (seq ["str1";"str3";"str2";"str4"])
        let expectedStr = seq ["str1";"str2";"str3";"str4"]
        VerifySeqsEqual expectedStr resultStr

        // empty Seq
        let resultEpt = Linq.sortWith intComparer Linq.empty
        VerifySeqsEqual resultEpt Linq.empty

        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.sortWith intComparer null  |> ignore)

        ()

    [<Test>]
    member this.Sum() =
    
        // integer Seq
        let resultInt = Linq.sum (seq [1..10])
        Assert.AreEqual(55,resultInt)
        
        // float32 Seq
        let floatSeq = (seq [ 1.2f;3.5f;6.7f ])
        let resultFloat = Linq.sum floatSeq
        if resultFloat <> 11.4f then Assert.Fail()
        
        // double Seq
        let doubleSeq = (seq [ 1.0;8.0 ])
        let resultDouble = Linq.sum doubleSeq
        if resultDouble <> 9.0 then Assert.Fail()
        
        // decimal Seq
        let decimalSeq = (seq [ 0M;19M;19.03M ])
        let resultDecimal = Linq.sum decimalSeq
        if resultDecimal <> 38.03M then Assert.Fail()      
          
      
        // empty float32 Seq
        let emptyFloatSeq = Linq.empty<System.Single> 
        let resultEptFloat = Linq.sum emptyFloatSeq 
        if resultEptFloat <> 0.0f then Assert.Fail()
        
        // empty double Seq
        let emptyDoubleSeq = Linq.empty<System.Double> 
        let resultDouEmp = Linq.sum emptyDoubleSeq 
        if resultDouEmp <> 0.0 then Assert.Fail()
        
        // empty decimal Seq
        let emptyDecimalSeq = Linq.empty<System.Decimal> 
        let resultDecEmp = Linq.sum emptyDecimalSeq 
        if resultDecEmp <> 0M then Assert.Fail()
       
        ()
        
    [<Test>]
    member this.SumBy() =

        // integer Seq
        let resultInt = Linq.sumBy int (seq [1..10])
        Assert.AreEqual(55,resultInt)
        
        // float32 Seq
        let floatSeq = (seq [ 1.2f;3.5f;6.7f ])
        let resultFloat = Linq.sumBy float32 floatSeq
        if resultFloat <> 11.4f then Assert.Fail()
        
        // double Seq
        let doubleSeq = (seq [ 1.0;8.0 ])
        let resultDouble = Linq.sumBy double doubleSeq
        if resultDouble <> 9.0 then Assert.Fail()
        
        // decimal Seq
        let decimalSeq = (seq [ 0M;19M;19.03M ])
        let resultDecimal = Linq.sumBy decimal decimalSeq
        if resultDecimal <> 38.03M then Assert.Fail()      

        // empty float32 Seq
        let emptyFloatSeq = Linq.empty<System.Single> 
        let resultEptFloat = Linq.sumBy float32 emptyFloatSeq 
        if resultEptFloat <> 0.0f then Assert.Fail()
        
        // empty double Seq
        let emptyDoubleSeq = Linq.empty<System.Double> 
        let resultDouEmp = Linq.sumBy double emptyDoubleSeq 
        if resultDouEmp <> 0.0 then Assert.Fail()
        
        // empty decimal Seq
        let emptyDecimalSeq = Linq.empty<System.Decimal> 
        let resultDecEmp = Linq.sumBy decimal emptyDecimalSeq 
        if resultDecEmp <> 0M then Assert.Fail()
       
        ()
        
    [<Test>]
    member this.Take() =
        // integer Seq
        
        let resultInt = Linq.take 3 (seq [1;2;4;5;7])
       
        let expectedInt = seq [1;2;4]
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
       
        let resultStr =Linq.take 2(seq ["str1";"str2";"str3";"str4"])
     
        let expectedStr = seq ["str1";"str2"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty Seq 
        let resultEpt = Linq.take 0 Linq.empty 
      
        VerifySeqsEqual resultEpt Linq.empty
        
         
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.take 1 null |> ignore)
        ()
        
    [<Test>]
    member this.takeWhile() =
        // integer Seq
        let funcInt x = (x < 6)
        let resultInt = Linq.takeWhile funcInt (seq [1;2;4;5;6;7])
      
        let expectedInt = seq [1;2;4;5]
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
        let funcStr (x:string) = (x.Length < 4)
        let resultStr =Linq.takeWhile funcStr (seq ["a"; "ab"; "abc"; "abcd"; "abcde"])
      
        let expectedStr = seq ["a"; "ab"; "abc"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty Seq 
        let resultEpt = Linq.takeWhile funcInt Linq.empty 
        VerifySeqsEqual resultEpt Linq.empty
        
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.takeWhile funcInt null |> ignore)
        ()
        
    [<Test>]
    member this.ToArray() =
        // integer Seq
        let resultInt = Linq.toArray(seq [1;2;4;5;7])
     
        let expectedInt = [|1;2;4;5;7|]
        Assert.AreEqual(expectedInt,resultInt)

        // string Seq
        let resultStr =Linq.toArray (seq ["str1";"str2";"str3"])
    
        let expectedStr =  [|"str1";"str2";"str3"|]
        Assert.AreEqual(expectedStr,resultStr)
        
        // empty Seq 
        let resultEpt = Linq.toArray Linq.empty 
        Assert.AreEqual([||],resultEpt)
        
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.toArray null |> ignore)
        ()
        
    [<Test>]    
    member this.ToArrayFromICollection() =
        let inputCollection = ResizeArray(seq [1;2;4;5;7])
        let resultInt = Linq.toArray(inputCollection)
        let expectedInt = [|1;2;4;5;7|]
        Assert.AreEqual(expectedInt,resultInt)        
    
    [<Test>]    
    member this.ToArrayEmptyInput() =
        let resultInt = Linq.toArray(Linq.empty<int>)
        let expectedInt = Array.empty<int>
        Assert.AreEqual(expectedInt,resultInt)        

    [<Test>]    
    member this.ToArrayFromArray() =
        let resultInt = Linq.toArray([|1;2;4;5;7|])
        let expectedInt = [|1;2;4;5;7|]
        Assert.AreEqual(expectedInt,resultInt)        
    
    [<Test>]    
    member this.ToArrayFromList() =
        let resultInt = Linq.toArray([1;2;4;5;7])
        let expectedInt = [|1;2;4;5;7|]
        Assert.AreEqual(expectedInt,resultInt)        

    [<Test>]
    member this.ToList() =
        // integer Seq
        let resultInt = Linq.toList (seq [1;2;4;5;7])
        let expectedInt = [1;2;4;5;7]
        Assert.AreEqual(expectedInt,resultInt)
        
        // string Seq
        let resultStr =Linq.toList (seq ["str1";"str2";"str3"])
        let expectedStr =  ["str1";"str2";"str3"]
        Assert.AreEqual(expectedStr,resultStr)
        
        // empty Seq 
        let resultEpt = Linq.toList Linq.empty 
        Assert.AreEqual([],resultEpt)
         
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.toList null |> ignore)
        ()

    [<Test>]
    member this.Transpose() =
        // integer seq
        VerifySeqsEqual [seq [1; 4]; seq [2; 5]; seq [3; 6]] <| Linq.transpose (seq [seq {1..3}; seq {4..6}])
        VerifySeqsEqual [seq [1]; seq [2]; seq [3]] <| Linq.transpose (seq [seq {1..3}])
        VerifySeqsEqual [seq {1..2}] <| Linq.transpose (seq [seq [1]; seq [2]])

        // string seq
        VerifySeqsEqual [seq ["a";"d"]; seq ["b";"e"]; seq ["c";"f"]] <| Linq.transpose (seq [seq ["a";"b";"c"]; seq ["d";"e";"f"]])

        // empty seq
        VerifySeqsEqual Linq.empty <| Linq.transpose Linq.empty

        // seq of empty seqs - m x 0 seq transposes to 0 x m (i.e. empty)
        VerifySeqsEqual Linq.empty <| Linq.transpose (seq [Linq.empty])
        VerifySeqsEqual Linq.empty <| Linq.transpose (seq [Linq.empty; Linq.empty])

        // null seq
        let nullSeq = null : seq<seq<string>>
        CheckThrowsArgumentNullException (fun () -> Linq.transpose nullSeq |> ignore)

        // sequences of lists
        VerifySeqsEqual [seq ["a";"c"]; seq ["b";"d"]] <| Linq.transpose [["a";"b"]; ["c";"d"]]
        VerifySeqsEqual [seq ["a";"c"]; seq ["b";"d"]] <| Linq.transpose (seq { yield ["a";"b"]; yield ["c";"d"] })

    [<Test>]
    member this.Truncate() =
        // integer Seq
        let resultInt = Linq.truncate 3 (seq [1;2;4;5;7])
        let expectedInt = [1;2;4]
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
        let resultStr =Linq.truncate 2 (seq ["str1";"str2";"str3"])
        let expectedStr =  ["str1";"str2"]
        VerifySeqsEqual expectedStr resultStr
        
        // empty Seq 
        let resultEpt = Linq.truncate 0 Linq.empty
        VerifySeqsEqual Linq.empty resultEpt
        
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.truncate 1 null |> ignore)

        // negative count
        VerifySeqsEqual Linq.empty <| Linq.truncate -1 (seq [1;2;4;5;7])
        VerifySeqsEqual Linq.empty <| Linq.truncate System.Int32.MinValue (seq [1;2;4;5;7])

        ()
        
    [<Test>]
    member this.tryFind() =
        // integer Seq
        let resultInt = Linq.tryFind (fun x -> (x%2=0)) (seq [1;2;4;5;7])
        Assert.AreEqual(Some(2), resultInt)
        
         // integer Seq - None
        let resultInt = Linq.tryFind (fun x -> (x%2=0)) (seq [1;3;5;7])
        Assert.AreEqual(None, resultInt)
        
        // string Seq
        let resultStr = Linq.tryFind (fun (x:string) -> x.Contains("2")) (seq ["str1";"str2";"str3"])
        Assert.AreEqual(Some("str2"),resultStr)
        
         // string Seq - None
        let resultStr = Linq.tryFind (fun (x:string) -> x.Contains("2")) (seq ["str1";"str4";"str3"])
        Assert.AreEqual(None,resultStr)
       
        
        // empty Seq 
        let resultEpt = Linq.tryFind (fun x -> (x%2=0)) Linq.empty
        Assert.AreEqual(None,resultEpt)

        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.tryFind (fun x -> (x%2=0))  null |> ignore)
        ()
        
    [<Test>]
    member this.TryFindBack() =
        // integer Seq
        let resultInt = Linq.tryFindBack (fun x -> (x%2=0)) (seq [1;2;4;5;7])
        Assert.AreEqual(Some 4, resultInt)

        // integer Seq - None
        let resultInt = Linq.tryFindBack (fun x -> (x%2=0)) (seq [1;3;5;7])
        Assert.AreEqual(None, resultInt)

        // string Seq
        let resultStr = Linq.tryFindBack (fun (x:string) -> x.Contains("2")) (seq ["str1";"str2";"str2x";"str3"])
        Assert.AreEqual(Some "str2x", resultStr)

        // string Seq - None
        let resultStr = Linq.tryFindBack (fun (x:string) -> x.Contains("2")) (seq ["str1";"str4";"str3"])
        Assert.AreEqual(None, resultStr)

        // empty Seq
        let resultEpt = Linq.tryFindBack (fun x -> (x%2=0)) Linq.empty
        Assert.AreEqual(None, resultEpt)

        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.tryFindBack (fun x -> (x%2=0))  null |> ignore)
        ()

    [<Test>]
    member this.TryFindIndex() =

        // integer Seq
        let resultInt = Linq.tryFindIndex (fun x -> (x % 5 = 0)) [8; 9; 10]
        Assert.AreEqual(Some(2), resultInt)
        
         // integer Seq - None
        let resultInt = Linq.tryFindIndex (fun x -> (x % 5 = 0)) [9;3;11]
        Assert.AreEqual(None, resultInt)
        
        // string Seq
        let resultStr = Linq.tryFindIndex (fun (x:string) -> x.Contains("2")) ["str1"; "str2"; "str3"]
        Assert.AreEqual(Some(1),resultStr)
        
         // string Seq - None
        let resultStr = Linq.tryFindIndex (fun (x:string) -> x.Contains("2")) ["str1"; "str4"; "str3"]
        Assert.AreEqual(None,resultStr)
       
        
        // empty Seq 
        let resultEpt = Linq.tryFindIndex (fun x -> (x%2=0)) Linq.empty
        Assert.AreEqual(None, resultEpt)
        
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.tryFindIndex (fun x -> (x % 2 = 0))  null |> ignore)
        ()
        
    [<Test>]
    member this.TryFindIndexBack() =

        // integer Seq
        let resultInt = Linq.tryFindIndexBack (fun x -> (x % 5 = 0)) [5; 9; 10; 12]
        Assert.AreEqual(Some(2), resultInt)

        // integer Seq - None
        let resultInt = Linq.tryFindIndexBack (fun x -> (x % 5 = 0)) [9;3;11]
        Assert.AreEqual(None, resultInt)

        // string Seq
        let resultStr = Linq.tryFindIndexBack (fun (x:string) -> x.Contains("2")) ["str1"; "str2"; "str2x"; "str3"]
        Assert.AreEqual(Some(2), resultStr)

        // string Seq - None
        let resultStr = Linq.tryFindIndexBack (fun (x:string) -> x.Contains("2")) ["str1"; "str4"; "str3"]
        Assert.AreEqual(None, resultStr)

        // empty Seq
        let resultEpt = Linq.tryFindIndexBack (fun x -> (x%2=0)) Linq.empty
        Assert.AreEqual(None, resultEpt)

        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.tryFindIndexBack (fun x -> (x % 2 = 0))  null |> ignore)
        ()

    [<Test>]
    member this.Unfold() =
        // integer Seq
        
        let resultInt = Linq.unfold (fun x -> if x = 1 then Some(7,2) else  None) 1
        
        VerifySeqsEqual (seq [7]) resultInt
          
        // string Seq
        let resultStr =Linq.unfold (fun (x:string) -> if x.Contains("unfold") then Some("a","b") else None) "unfold"
        VerifySeqsEqual (seq ["a"]) resultStr
        ()
        
        
    [<Test>]
    member this.Windowed() =

        let testWindowed config =
            try
                config.InputSeq
                |> Linq.windowed config.WindowSize
                |> VerifySeqsEqual config.ExpectedSeq 
            with
            | _ when Option.isNone config.Exception -> Assert.Fail()
            | e when e.GetType() = (Option.get config.Exception) -> ()
            | _ -> Assert.Fail()

        {
          InputSeq = seq [1..10]
          WindowSize = 1
          ExpectedSeq =  seq { for i in 1..10 do yield [| i |] }
          Exception = None
        } |> testWindowed
        {
          InputSeq = seq [1..10]
          WindowSize = 5
          ExpectedSeq =  seq { for i in 1..6 do yield [| i; i+1; i+2; i+3; i+4 |] }
          Exception = None
        } |> testWindowed
        {
          InputSeq = seq [1..10]
          WindowSize = 10
          ExpectedSeq =  seq { yield [| 1 .. 10 |] }
          Exception = None
        } |> testWindowed
        {
          InputSeq = seq [1..10]
          WindowSize = 25
          ExpectedSeq =  Linq.empty
          Exception = None
        } |> testWindowed
        {
          InputSeq = seq ["str1";"str2";"str3";"str4"]
          WindowSize = 2
          ExpectedSeq =  seq [ [|"str1";"str2"|];[|"str2";"str3"|];[|"str3";"str4"|]]
          Exception = None
        } |> testWindowed
        {
          InputSeq = Linq.empty
          WindowSize = 2
          ExpectedSeq = Linq.empty
          Exception = None
        } |> testWindowed
        {
          InputSeq = null
          WindowSize = 2
          ExpectedSeq = Linq.empty
          Exception = Some typeof<ArgumentNullException>
        } |> testWindowed
        {
          InputSeq = seq [1..10]
          WindowSize = 0
          ExpectedSeq =  Linq.empty
          Exception = Some typeof<ArgumentException>
        } |> testWindowed

        ()
        
    [<Test>]
    member this.Zip() =
    
        // integer Seq
        let resultInt = Linq.zip (seq [1..7]) (seq [11..17])
        let expectedInt = 
            seq { for i in 1..7 do
                    yield i, i+10 }
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
        let resultStr =Linq.zip (seq ["str3";"str4"]) (seq ["str1";"str2"])
        let expectedStr = seq ["str3","str1";"str4","str2"]
        VerifySeqsEqual expectedStr resultStr
      
        // empty Seq 
        let resultEpt = Linq.zip Linq.empty Linq.empty
        VerifySeqsEqual Linq.empty resultEpt
          
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.zip null null |> ignore)
        CheckThrowsArgumentNullException(fun() -> Linq.zip null (seq [1..7]) |> ignore)
        CheckThrowsArgumentNullException(fun() -> Linq.zip (seq [1..7]) null |> ignore)
        ()
        
    [<Test>]
    member this.Zip3() =
        // integer Seq
        let resultInt = Linq.zip3 (seq [1..7]) (seq [11..17]) (seq [21..27])
        let expectedInt = 
            seq { for i in 1..7 do
                    yield i, (i + 10), (i + 20) }
        VerifySeqsEqual expectedInt resultInt
        
        // string Seq
        let resultStr =Linq.zip3 (seq ["str1";"str2"]) (seq ["str11";"str12"]) (seq ["str21";"str22"])
        let expectedStr = seq ["str1","str11","str21";"str2","str12","str22" ]
        VerifySeqsEqual expectedStr resultStr
      
        // empty Seq 
        let resultEpt = Linq.zip3 Linq.empty Linq.empty Linq.empty
        VerifySeqsEqual Linq.empty resultEpt
          
        // null Seq
        CheckThrowsArgumentNullException(fun() -> Linq.zip3 null null null |> ignore)
        CheckThrowsArgumentNullException(fun() -> Linq.zip3 null (seq [1..7]) (seq [1..7]) |> ignore)
        CheckThrowsArgumentNullException(fun() -> Linq.zip3 (seq [1..7]) null (seq [1..7]) |> ignore)
        CheckThrowsArgumentNullException(fun() -> Linq.zip3 (seq [1..7]) (seq [1..7]) null |> ignore)
        ()
        
    [<Test>]
    member this.tryPick() =
         // integer Seq
        let resultInt = Linq.tryPick (fun x-> if x = 1 then Some("got") else None) (seq [1..5])
         
        Assert.AreEqual(Some("got"),resultInt)
        
        // string Seq
        let resultStr = Linq.tryPick (fun x-> if x = "Are" then Some("got") else None) (seq ["Lists"; "Are"])
        Assert.AreEqual(Some("got"),resultStr)
        
        // empty Seq   
        let resultEpt = Linq.tryPick (fun x-> if x = 1 then Some("got") else None) Linq.empty
        Assert.IsNull(resultEpt)
       
        // null Seq
        let nullSeq : seq<'a> = null 
        let funcNull x = Some(1)
        
        CheckThrowsArgumentNullException(fun () -> Linq.tryPick funcNull nullSeq |> ignore)
   
        ()

    [<Test>]
    member this.tryItem() =
        // integer Seq
        let resultInt = Linq.tryItem 3 { 10..20 }
        Assert.AreEqual(Some(13), resultInt)

        // string Seq
        let resultStr = Linq.tryItem 2 (seq ["Lists"; "Are"; "Cool"; "List" ])
        Assert.AreEqual(Some("Cool"), resultStr)

        // empty Seq
        let resultEmpty = Linq.tryItem 0 Linq.empty
        Assert.AreEqual(None, resultEmpty)

        // null Seq
        let nullSeq:seq<'a> = null
        CheckThrowsArgumentNullException (fun () -> Linq.tryItem 3 nullSeq |> ignore)

        // Negative index
        let resultNegativeIndex = Linq.tryItem -1 { 10..20 }
        Assert.AreEqual(None, resultNegativeIndex)

        // Index greater than length
        let resultIndexGreater = Linq.tryItem 31 { 10..20 }
        Assert.AreEqual(None, resultIndexGreater)
