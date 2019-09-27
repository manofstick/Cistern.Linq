module Cistern.Linq.FSharp.Comparison

// This is a subset of the file
// https://github.com/manofstick/visualfsharp/blob/notagged/src/fsharp/FSharp.Core/prim-types.fs
// which was modified in the PR https://github.com/dotnet/fsharp/pull/5112
// that was never merged to the main line.

open System.Collections
open System
open System.Collections.Generic
open System.Reflection

let inline byteEq (x:byte) (y:byte) = (# "ceq" x y : bool #) 
let inline int32Eq (x:int32) (y:int32) = (# "ceq" x y : bool #) 
let inline float32Eq (x:float32) (y:float32) = (# "ceq" x y : bool #) 
let inline floatEq (x:float) (y:float) = (# "ceq" x y : bool #) 
let inline charEq (x:char) (y:char) = (# "ceq" x y : bool #) 
let inline int64Eq (x:int64) (y:int64) = (# "ceq" x y : bool #) 
let inline objEq (xobj:obj) (yobj:obj) = (# "ceq" xobj yobj : bool #)

let inline intOfByte (b:byte) = (# "" b : int #)

let inline intOrder (x:int) (y:int) = if (# "clt" x y : bool #) then (0-1) else (# "cgt" x y : int #)
let inline int64Order (x:int64) (y:int64) = if (# "clt" x y : bool #) then (0-1) else (# "cgt" x y : int #)

let inline unsafeDefault<'T> : 'T = Unchecked.defaultof<'T> //(# "ilzero !0" type ('T) : 'T #)
let inline get (arr:'T[]) (n:int) = arr.[n] //(# "ldelem.any !0" type ('T) arr n : 'T #)
let inline set (arr:'T[]) (n:int) (x:'T) = arr.[n] <- x //(# "stelem.any !0" type ('T) arr n x #)
let inline zeroCreate (n:int) = Array.zeroCreate n //(# "newarr !0" type ('T) n : 'T[] #)
//let inline unboxPrim<'T>(x:obj) = unbox x//(# "unbox.any !0" type ('T) x : 'T #)

let inline (>=.)   (x:int64) (y:int64)  = not(# "clt" x y : bool #)
let inline (+.)    (x:int64)  (y:int64)  = (# "add" x y : int64 #)

module internal Reflection =
    let inline flagsOr<'a> (lhs:'a) (rhs:'a) =
        (# "or" lhs rhs : 'a #)

    let inline flagsAnd<'a> (lhs:'a) (rhs:'a) =
        (# "and" lhs rhs : 'a #)

    let inline flagsContains<'a when 'a : equality> (flags:'a) (mask:'a) (value:'a) =
        (flagsAnd flags mask).Equals value

    let inline flagsIsSet<'a when 'a : equality> (flags:'a) (value:'a) =
        flagsContains flags value value

#if FX_RESHAPED_REFLECTION
    module internal ReflectionAdapters =
        let toArray<'a> (s:System.Collections.Generic.IEnumerable<'a>) =
            (System.Collections.Generic.List<'a> s).ToArray ()

        open System

        let inline hasFlag (flag : BindingFlags) f  = flagsIsSet f flag
        let isDeclaredFlag  f    = hasFlag BindingFlags.DeclaredOnly f
        let isPublicFlag    f    = hasFlag BindingFlags.Public f
        let isStaticFlag    f    = hasFlag BindingFlags.Static f
        let isInstanceFlag  f    = hasFlag BindingFlags.Instance f
        let isNonPublicFlag f    = hasFlag BindingFlags.NonPublic f

        let isAcceptable bindingFlags isStatic isPublic =
            // 1. check if member kind (static\instance) was specified in flags
            ((isStaticFlag bindingFlags && isStatic) || (isInstanceFlag bindingFlags && not isStatic)) && 
            // 2. check if member accessibility was specified in flags
            ((isPublicFlag bindingFlags && isPublic) || (isNonPublicFlag bindingFlags && not isPublic))

        type System.Type with
            member this.GetNestedType (name, bindingFlags) = 
                // MSDN: http://msdn.microsoft.com/en-us/library/0dcb3ad5.aspx
                // The following BindingFlags filter flags can be used to define which nested types to include in the search:
                // You must specify either BindingFlags.Public or BindingFlags.NonPublic to get a return.
                // Specify BindingFlags.Public to include public nested types in the search.
                // Specify BindingFlags.NonPublic to include non-public nested types (that is, private, internal, and protected nested types) in the search.
                // This method returns only the nested types of the current type. It does not search the base classes of the current type. 
                // To find types that are nested in base classes, you must walk the inheritance hierarchy, calling GetNestedType at each level.
                let e = this.GetTypeInfo().DeclaredNestedTypes.GetEnumerator ()
                let rec f () =
                    if not (e.MoveNext ()) then null
                    else
                        let nestedTy = e.Current
                        if (String.Equals (nestedTy.Name, name)) &&
                               ((isPublicFlag bindingFlags && nestedTy.IsNestedPublic) || 
                                (isNonPublicFlag bindingFlags && (nestedTy.IsNestedPrivate || nestedTy.IsNestedFamily || nestedTy.IsNestedAssembly || nestedTy.IsNestedFamORAssem || nestedTy.IsNestedFamANDAssem))) then
                            nestedTy.AsType ()
                        else
                            f ()
                f ()

            // use different sources based on Declared flag
            member this.GetMethods (bindingFlags) =
                let methods = 
                    if isDeclaredFlag bindingFlags then
                        this.GetTypeInfo().DeclaredMethods
                    else
                        this.GetRuntimeMethods()

                Array.FindAll (toArray methods, Predicate (fun m ->
                    isAcceptable bindingFlags m.IsStatic m.IsPublic))

            // use different sources based on Declared flag
            member this.GetFields (bindingFlags) = 
                let fields = 
                    if isDeclaredFlag bindingFlags then
                        this.GetTypeInfo().DeclaredFields
                    else
                        this.GetRuntimeFields()

                Array.FindAll (toArray fields, Predicate (fun f ->
                    isAcceptable bindingFlags f.IsStatic f.IsPublic))

            // use different sources based on Declared flag
            member this.GetProperties (bindingFlags) = 
                let properties =
                    if isDeclaredFlag bindingFlags then
                        this.GetTypeInfo().DeclaredProperties
                    else
                        this.GetRuntimeProperties ()

                Array.FindAll (toArray properties, Predicate (fun pi -> 
                    let mi =
                        match pi.GetMethod with
                        | null -> pi.SetMethod
                        | _    -> pi.GetMethod
                    if obj.ReferenceEquals (mi, null) then
                        false
                    else
                        isAcceptable bindingFlags mi.IsStatic mi.IsPublic))

            member this.IsGenericTypeDefinition = this.GetTypeInfo().IsGenericTypeDefinition
                
        type System.Reflection.MemberInfo with
            member this.GetCustomAttributes(attrTy, inherits) : obj[] =
                downcast box(toArray (CustomAttributeExtensions.GetCustomAttributes(this, attrTy, inherits)))

    module internal SystemAdapters =
        type Converter<'TInput, 'TOutput> = delegate of 'TInput -> 'TOutput

        type System.Array with
            static member ConvertAll<'TInput, 'TOutput>(input:'TInput[], conv:Converter<'TInput, 'TOutput>) =
                let output = (# "newarr !0" type ('TOutput) input.Length : 'TOutput array #)
                for i = 0 to input.Length-1 do
                    set output i (conv.Invoke (get input i))
                output

    open PrimReflectionAdapters
    open ReflectionAdapters
    open SystemAdapters
#endif


#if FX_RESHAPED_REFLECTION
    let instancePropertyFlags = BindingFlags.Instance 
    let staticFieldFlags = BindingFlags.Static 
    let staticMethodFlags = BindingFlags.Static 
#else    
    let instancePropertyFlags = flagsOr BindingFlags.GetProperty BindingFlags.Instance 
    let staticFieldFlags = flagsOr BindingFlags.GetField BindingFlags.Static 
    let staticMethodFlags = BindingFlags.Static 
#endif

    let tupleNames = [|
        "System.Tuple`1";      "System.Tuple`2";      "System.Tuple`3";
        "System.Tuple`4";      "System.Tuple`5";      "System.Tuple`6";
        "System.Tuple`7";      "System.Tuple`8";      "System.Tuple"
        "System.ValueTuple`1"; "System.ValueTuple`2"; "System.ValueTuple`3";
        "System.ValueTuple`4"; "System.ValueTuple`5"; "System.ValueTuple`6";
        "System.ValueTuple`7"; "System.ValueTuple`8"; "System.ValueTuple" |]

    let simpleTupleNames = [|
        "Tuple`1";      "Tuple`2";      "Tuple`3";
        "Tuple`4";      "Tuple`5";      "Tuple`6";
        "Tuple`7";      "Tuple`8";      
        "ValueTuple`1"; "ValueTuple`2"; "ValueTuple`3";
        "ValueTuple`4"; "ValueTuple`5"; "ValueTuple`6";
        "ValueTuple`7"; "ValueTuple`8"; |]

    let isTupleType (typ:Type) = 
      // We need to be careful that we only rely typ.IsGenericType, typ.Namespace and typ.Name here.
      //
      // Historically the FSharp.Core reflection utilities get used on implementations of 
      // System.Type that don't have functionality such as .IsEnum and .FullName fully implemented.
      // This happens particularly over TypeBuilderInstantiation types in the ProvideTypes implementation of System.TYpe
      // used in F# type providers.
      typ.IsGenericType &&
      System.String.Equals(typ.Namespace, "System") && 
      Array.Exists (simpleTupleNames, Predicate typ.Name.StartsWith)

#if !FX_NO_REFLECTION_ONLY
    let assemblyName = typeof<CompilationMappingAttribute>.Assembly.GetName().Name 
    let _ = assert (System.String.Equals (assemblyName, "FSharp.Core"))
    let cmaName = typeof<CompilationMappingAttribute>.FullName
    
    let tryFindCompilationMappingAttributeFromData (attrs:System.Collections.Generic.IList<CustomAttributeData>, res:byref<SourceConstructFlags*int*int>) : bool =
        match attrs with
        | null -> false
        | _ -> 
            let mutable found = false
            for a in attrs do
                if a.Constructor.DeclaringType.FullName.Equals cmaName then 
                    let args = a.ConstructorArguments
                    let flags = 
                         match args.Count  with 
                         | 1 -> ((let x = args.[0] in x.Value :?> SourceConstructFlags), 0, 0)
                         | 2 -> ((let x = args.[0] in x.Value :?> SourceConstructFlags), (let x = args.[1] in x.Value :?> int), 0)
                         | 3 -> ((let x = args.[0] in x.Value :?> SourceConstructFlags), (let x = args.[1] in x.Value :?> int), (let x = args.[2] in x.Value :?> int))
                         | _ -> (SourceConstructFlags.None, 0, 0)
                    res <- flags
                    found <- true
            found

    let findCompilationMappingAttributeFromData attrs =
        let mutable x = unsafeDefault<_>
        match tryFindCompilationMappingAttributeFromData (attrs, &x) with
        | false -> raise (Exception "no compilation mapping attribute")
        | true -> x
#endif
    let hasCustomEquality (typ:Type) =
        let arr = typ.GetCustomAttributes (typeof<CustomEqualityAttribute>, false)
        arr.Length > 0

    let hasCustomComparison (typ:Type) =
        let arr = typ.GetCustomAttributes (typeof<CustomComparisonAttribute>, false)
        arr.Length > 0

    let tryFindCompilationMappingAttribute (attrs:obj[], res:byref<SourceConstructFlags*int*int>) : bool =
      match attrs with
      | null | [||] -> false
      | [| :? CompilationMappingAttribute as a |] ->
        res <- a.SourceConstructFlags, a.SequenceNumber, a.VariantNumber
        true
      | _ -> failwith "raise (System.InvalidOperationException (SR.GetString(SR.multipleCompilationMappings)))"

    let findCompilationMappingAttribute (attrs:obj[]) =
        let mutable x = unsafeDefault<_>
        match tryFindCompilationMappingAttribute (attrs, &x) with
        | false -> raise (Exception "no compilation mapping attribute")
        | true -> x

    let tryFindCompilationMappingAttributeFromType (typ:Type, res:byref<SourceConstructFlags*int*int>) : bool =
#if !FX_NO_REFLECTION_ONLY
        let assem = typ.Assembly
        if (not (obj.ReferenceEquals(assem, null))) && assem.ReflectionOnly then 
           tryFindCompilationMappingAttributeFromData (typ.GetCustomAttributesData(), &res)
        else
#endif
        tryFindCompilationMappingAttribute (typ.GetCustomAttributes (typeof<CompilationMappingAttribute>,false), &res)

    let tryFindCompilationMappingAttributeFromMemberInfo (info:MemberInfo, res:byref<SourceConstructFlags*int*int>) : bool =
#if !FX_NO_REFLECTION_ONLY
        let assem = info.DeclaringType.Assembly
        if (not (obj.ReferenceEquals (assem, null))) && assem.ReflectionOnly then 
           tryFindCompilationMappingAttributeFromData (info.GetCustomAttributesData(), &res)
        else
#endif
        tryFindCompilationMappingAttribute (info.GetCustomAttributes (typeof<CompilationMappingAttribute>,false), &res)

    let findCompilationMappingAttributeFromMemberInfo (info:MemberInfo) =    
#if !FX_NO_REFLECTION_ONLY
        let assem = info.DeclaringType.Assembly
        if (not (obj.ReferenceEquals (assem, null))) && assem.ReflectionOnly then 
            findCompilationMappingAttributeFromData (info.GetCustomAttributesData())
        else
#endif
        findCompilationMappingAttribute (info.GetCustomAttributes (typeof<CompilationMappingAttribute>,false))

    let tryFindSourceConstructFlagsOfType (typ:Type, res:byref<SourceConstructFlags>) : bool =
      let mutable x = unsafeDefault<_>
      if tryFindCompilationMappingAttributeFromType (typ, &x) then
        let flags,_n,_vn = x
        res <- flags
        true
      else
        false

    let isKnownType (typ:Type, bindingFlags:BindingFlags, knownType:SourceConstructFlags) =
        let mutable flags = unsafeDefault<_>
        match tryFindSourceConstructFlagsOfType (typ, &flags) with 
        | false -> false 
        | true ->
          (flagsContains flags SourceConstructFlags.KindMask knownType) &&
          // We see private representations only if BindingFlags.NonPublic is set
          (if flagsIsSet flags SourceConstructFlags.NonPublicRepresentation then 
              flagsIsSet bindingFlags BindingFlags.NonPublic
           else 
              true)

    let isRecordType (typ:Type, bindingFlags:BindingFlags) = isKnownType (typ, bindingFlags, SourceConstructFlags.RecordType)
    let isObjectType (typ:Type, bindingFlags:BindingFlags) = isKnownType (typ, bindingFlags, SourceConstructFlags.ObjectType)
    let isUnionType  (typ:Type, bindingFlags:BindingFlags) = isKnownType (typ, bindingFlags, SourceConstructFlags.SumType)

    let isFieldProperty (prop : PropertyInfo) =
        let mutable res = unsafeDefault<_>
        match tryFindCompilationMappingAttributeFromMemberInfo(prop:>MemberInfo, &res) with
        | false -> false
        | true -> 
            let (flags,_n,_vn) = res
            flagsContains flags SourceConstructFlags.KindMask SourceConstructFlags.Field

    let sequenceNumberOfMember (x:MemberInfo) = let (_,n,_)  = findCompilationMappingAttributeFromMemberInfo x in n
    let variantNumberOfMember  (x:MemberInfo) = let (_,_,vn) = findCompilationMappingAttributeFromMemberInfo x in vn

    // Although this funciton is called sortFreshArray (and was so in it's previously life in reflect.fs)
    // it does not create a fresh array, but rather uses the existing array.
    let sortFreshArray (f:'a->int) (arr:'a[]) =
        let comparer = System.Collections.Generic.Comparer<int>.Default
        System.Array.Sort (arr, {
            new IComparer<'a> with
                member __.Compare (lhs:'a, rhs:'a) =
                    comparer.Compare (f lhs, f rhs) })
        arr

    let fieldPropsOfRecordType (typ:Type, bindingFlags) =
        let properties = typ.GetProperties (flagsOr instancePropertyFlags bindingFlags) 
        let fields = System.Array.FindAll (properties, Predicate isFieldProperty)
        sortFreshArray sequenceNumberOfMember fields

    let getUnionTypeTagNameMap (typ:Type,bindingFlags:BindingFlags) : (int*string)[] = 
        let enumTyp = typ.GetNestedType ("Tags", bindingFlags)
        // Unions with a singleton case do not get a Tags type (since there is only one tag), hence enumTyp may be null in this case
        match enumTyp with
        | null -> 
            let methods = typ.GetMethods (flagsOr staticMethodFlags bindingFlags) 
            let maybeTagNames = 
                Array.ConvertAll (methods, Converter (fun minfo -> 
                    let mutable res = unsafeDefault<_>
                    match tryFindCompilationMappingAttributeFromMemberInfo (minfo:>MemberInfo, &res) with
                    | false -> unsafeDefault<_>
                    | true -> 
                        let flags,n,_vn = res
                        if flagsContains flags SourceConstructFlags.KindMask SourceConstructFlags.UnionCase then 
                            // chop "get_" or  "New" off the front 
                            let nm = 
                                let nm = minfo.Name
                                if   nm.StartsWith "get_" then nm.Substring 4
                                elif nm.StartsWith "New"  then nm.Substring 3
                                else nm
                            (n, nm)
                        else
                            unsafeDefault<_> ))
            Array.FindAll (maybeTagNames, Predicate (fun maybeTagName -> not (obj.ReferenceEquals (maybeTagName, null))))
        | _ -> 
            let fields = enumTyp.GetFields (flagsOr staticFieldFlags bindingFlags) 
            let filtered = Array.FindAll (fields, (fun (f:FieldInfo) -> f.IsStatic && f.IsLiteral))
            let sorted = sortFreshArray (fun (f:FieldInfo) -> (f.GetValue null) :?> int) filtered
            Array.ConvertAll (sorted, Converter (fun tagfield -> (tagfield.GetValue(null) :?> int),tagfield.Name))

    let getUnionCasesTyp (typ: Type, _bindingFlags) = 
#if CASES_IN_NESTED_CLASS
        let casesTyp = typ.GetNestedType("Cases", bindingFlags)
        if casesTyp.IsGenericTypeDefinition then casesTyp.MakeGenericType(typ.GetGenericArguments())
        else casesTyp
#else
        typ
#endif

    let getUnionCaseTyp (typ: Type, tag: int, bindingFlags) = 
        let tagFields = getUnionTypeTagNameMap(typ,bindingFlags)
        let tagField = let _,f = Array.Find (tagFields, Predicate (fun (i,_) -> i = tag)) in f
            
        if tagFields.Length = 1 then 
            typ
        else
            // special case: two-cased DU annotated with CompilationRepresentation(UseNullAsTrueValue)
            // in this case it will be compiled as one class: return self type for non-nullary case and null for nullary
            let isTwoCasedDU =
                if tagFields.Length = 2 then
                    match typ.GetCustomAttributes(typeof<CompilationRepresentationAttribute>, false) with
                    | [|:? CompilationRepresentationAttribute as attr|] -> 
                        flagsIsSet attr.Flags CompilationRepresentationFlags.UseNullAsTrueValue
                    | _ -> false
                else
                    false
            if isTwoCasedDU then
                typ
            else
            let casesTyp = getUnionCasesTyp (typ, bindingFlags)
            let caseTyp = casesTyp.GetNestedType(tagField, bindingFlags) // if this is null then the union is nullary
            match caseTyp with 
            | null -> null
            | _ when caseTyp.IsGenericTypeDefinition -> caseTyp.MakeGenericType(casesTyp.GetGenericArguments())
            | _ -> caseTyp

    let fieldsPropsOfUnionCase(typ:Type, tag:int, bindingFlags) =
        // Lookup the type holding the fields for the union case
        let caseTyp = getUnionCaseTyp (typ, tag, bindingFlags)
        let caseTyp = match caseTyp with null ->  typ | _ -> caseTyp
        let properties = caseTyp.GetProperties (flagsOr instancePropertyFlags bindingFlags) 
        let filtered = Array.FindAll (properties, Predicate (fun p -> if isFieldProperty p then (variantNumberOfMember (p:>MemberInfo)) = tag else false))
        sortFreshArray (fun (p:PropertyInfo) -> sequenceNumberOfMember p) filtered

    let getAllInstanceFields (typ:Type) =
        let fields = typ.GetFields (flagsOr BindingFlags.Instance (flagsOr BindingFlags.Public BindingFlags.NonPublic))
        Array.ConvertAll (fields, Converter (fun p -> p.FieldType))




module HashCompare =
#if FX_RESHAPED_REFLECTION
    open Reflection.SystemAdapters
#endif
    let isArray (ty:Type) =
        ty.IsArray || (typeof<System.Array>.IsAssignableFrom ty)

    let canUseDotnetDefaultComparisonOrEquality isCustom hasStructuralInterface stringsRequireHandling er (rootType:Type) =
        let processed = System.Collections.Generic.HashSet ()

        let bindingPublicOrNonPublic =
            Reflection.flagsOr BindingFlags.Public BindingFlags.NonPublic

        let rec isSuitableNullableTypeOrNotNullable (ty:Type) =
            // although nullables not explicitly handled previously, they need special handling
            // due to the implicit casting to their underlying generic type (i.e. could be a float)
            let isNullableType = 
                ty.IsGenericType
                && ty.GetGenericTypeDefinition().Equals typedefof<Nullable<_>>
            if isNullableType then
                checkType 0 (ty.GetGenericArguments ())
            else 
                true

        and isSuitableTupleType (ty:Type) =
            ty.IsValueType && // Tuple<...> don't have implementation, but ValueTuple<...> does
            Reflection.isTupleType ty && 
            checkType 0 (ty.GetGenericArguments ())

        and isSuitableStructType (ty:Type) =
            ty.IsValueType &&
            Reflection.isObjectType (ty, bindingPublicOrNonPublic) &&
            (not (isCustom ty)) &&
            checkType 0 (Reflection.getAllInstanceFields ty)

        and isSuitableRecordType (ty:Type) =
            Reflection.isRecordType (ty, bindingPublicOrNonPublic) &&
            (not (isCustom ty)) &&
            ( let fields = Reflection.fieldPropsOfRecordType (ty, bindingPublicOrNonPublic)
              let fieldTypes = Array.ConvertAll (fields, Converter (fun f -> f.PropertyType))
              checkType 0 fieldTypes)

        and isSuitableUnionType (ty:Type) =
            Reflection.isUnionType (ty, bindingPublicOrNonPublic) &&
            (not (isCustom ty)) &&
            ( let cases = Reflection.getUnionTypeTagNameMap (ty, bindingPublicOrNonPublic)
              let rec checkCases idx =
                  if idx = cases.Length then true
                  else
                      let tag,_ = cases.[idx]
                      let fields = Reflection.fieldsPropsOfUnionCase (ty, tag, bindingPublicOrNonPublic)
                      let fieldTypes = Array.ConvertAll (fields, Converter (fun f -> f.PropertyType))
                      if checkType 0 fieldTypes then
                          checkCases (idx+1)
                      else
                          false
              checkCases 0)

        and checkType idx (types:Type[]) =
            if idx = types.Length then true
            else
                let ty = types.[idx]
                if not (processed.Add ty) then
                    checkType (idx+1) types
                else
                    ty.IsSealed // covers enum and value types; derived ref types might implement from hasStructuralInterface
                    && not (isArray ty)
                    && (not stringsRequireHandling || (not (ty.Equals typeof<string>)))
                    && (er || (not (ty.Equals typeof<float>)))
                    && (er || (not (ty.Equals typeof<float32>)))
                    && isSuitableNullableTypeOrNotNullable ty
                    && ((not (hasStructuralInterface ty))
                        || isSuitableTupleType ty
                        || isSuitableStructType ty
                        || isSuitableRecordType ty
                        || isSuitableUnionType ty)
                    && checkType (idx+1) types

        checkType 0 [|rootType|]

    //-------------------------------------------------------------------------
    // LanguagePrimitives.HashCompare: HASHING.  
    //------------------------------------------------------------------------- 
    let defaultHashNodes = 18 

    /// The implementation of IEqualityComparer, using depth-limited for hashing and PER semantics for NaN equality.
    type CountLimitedHasherPER(sz:int) =
        [<DefaultValue>]
        val mutable nodeCount : int
        
        member x.Fresh() = 
            if (System.Threading.Interlocked.CompareExchange(&(x.nodeCount), sz, 0) = 0) then 
                x
            else
                new CountLimitedHasherPER(sz)
        
        interface IEqualityComparer 

    /// The implementation of IEqualityComparer, using unlimited depth for hashing and ER semantics for NaN equality.
    type UnlimitedHasherER() =
        interface IEqualityComparer 
        
    /// The implementation of IEqualityComparer, using unlimited depth for hashing and PER semantics for NaN equality.
    type UnlimitedHasherPER() =
        interface IEqualityComparer
            

    /// The unique object for unlimited depth for hashing and ER semantics for equality.
    let fsEqualityComparerUnlimitedHashingER = UnlimitedHasherER()

    /// The unique object for unlimited depth for hashing and PER semantics for equality.
    let fsEqualityComparerUnlimitedHashingPER = UnlimitedHasherPER()
             
    let inline HashCombine nr x y = (x <<< 1) + y + 631 * nr

    let inline ArrayHashing<'element,'array when 'array :> System.Array> get lowerBound (f:'element->int) (x:'array) : int =
        let rec loop acc i =
            if i < lowerBound then acc
            else loop (HashCombine i acc (f (get x i))) (i-1)
        
        let lastIdx =
            let upperBound = lowerBound+defaultHashNodes
            match lowerBound+x.Length-1 with
            | oversized when oversized > upperBound -> upperBound
            | good -> good

        loop 0 lastIdx 

    let GenericHashObjArray   (iec:System.Collections.IEqualityComparer) (x:obj[]) = ArrayHashing get 0 iec.GetHashCode x
    let GenericHashByteArray  (x:byte[])  = ArrayHashing get 0 intOfByte x
    let GenericHashInt32Array (x:int32[]) = ArrayHashing get 0 (fun x -> x) x
    let GenericHashInt64Array (x:int64[]) = ArrayHashing get 0 int32 x

    // special case - arrays do not by default have a decent structural hashing function
    let GenericHashArbArray (iec : System.Collections.IEqualityComparer) (x: System.Array) : int =
        match x.Rank with 
        | 1 -> ArrayHashing (fun a i -> a.GetValue i) (x.GetLowerBound 0) iec.GetHashCode x
        | _ -> HashCombine 10 (x.GetLength(0)) (x.GetLength(1)) 

    // Core implementation of structural hashing, corresponds to pseudo-code in the 
    // F# Language spec.  Searches for the IStructuralHash interface, otherwise uses GetHashCode().
    // Arrays are structurally hashed through a separate technique.
    //
    // "iec" is either fsEqualityComparerUnlimitedHashingER, fsEqualityComparerUnlimitedHashingPER or a CountLimitedHasherPER.
    let rec GenericHashParamObj (iec : System.Collections.IEqualityComparer) (x: obj) : int =
          match x with 
          | null -> 0 
          | (:? System.Array as a) -> 
              // due to the rules of the CLI type system, array casts are "assignment compatible"
              // see: https://blogs.msdn.microsoft.com/ericlippert/2009/09/24/why-is-covariance-of-value-typed-arrays-inconsistent/
              // this means that the cast and comparison for byte will also handle sbyte, int32 handle uint32, 
              // and int64 handle uint64. The hash code of an individual array element is different for the different
              // types, but it is irrelevant for the creation of the hash code - but this is to be replicated in
              // the tryGetFSharpArrayEqualityComparer function.
              match a with 
              | :? (obj[]) as oa -> GenericHashObjArray iec oa 
              | :? (byte[]) as ba -> GenericHashByteArray ba 
              | :? (int[]) as ba -> GenericHashInt32Array ba 
              | :? (int64[]) as ba -> GenericHashInt64Array ba 
              | _ -> GenericHashArbArray iec a 
          | :? IStructuralEquatable as a ->    
              a.GetHashCode(iec)
          | _ -> 
              x.GetHashCode()

    /// Direct call to GetHashCode on the string type
    let inline HashString (s:string) = 
         match s with 
         | null -> 0 
         | _ -> (# "call instance int32 [mscorlib]System.String::GetHashCode()" s : int #)
            
    // from mscorlib v4.0.30319
    let inline HashChar (x:char) = (# "or" (# "shl" x 16 : int #) x : int #)
    let inline HashSByte (x:sbyte) = (# "xor" (# "shl" x 8 : int #) x : int #)
    let inline HashInt16 (x:int16) = (# "or" (# "conv.u2" x : int #) (# "shl" x 16 : int #) : int #)
    let inline HashInt64 (x:int64) = (# "xor" (# "conv.i4" x : int #) (# "conv.i4" (# "shr" x 32 : int #) : int #) : int #)
    let inline HashUInt64 (x:uint64) = (# "xor" (# "conv.i4" x : int #) (# "conv.i4" (# "shr.un" x 32 : int #) : int #) : int #)
    let inline HashIntPtr (x:nativeint) = (# "conv.i4" (# "conv.u8" x : uint64 #) : int #)
    let inline HashUIntPtr (x:unativeint) = (# "and" (# "conv.i4" (# "conv.u8" x : uint64 #) : int #) 0x7fffffff : int #)


    //-------------------------------------------------------------------------
    // LanguagePrimitives.HashCompare: Physical Equality
    //------------------------------------------------------------------------- 

    // NOTE: compiler/optimizer is aware of this function and optimizes calls to it in many situations
    // where it is known that PhysicalEqualityObj is identical to reference comparison
    let PhysicalEqualityIntrinsic (x:'T) (y:'T) : bool when 'T : not struct = 
        objEq (box x) (box y)

    let inline PhysicalEqualityFast (x:'T) (y:'T) : bool when 'T : not struct  = 
        PhysicalEqualityIntrinsic x y
  
    let PhysicalHashIntrinsic (input: 'T) : int when 'T : not struct  = 
        System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(box input)

    let inline PhysicalHashFast (input: 'T) = 
        PhysicalHashIntrinsic input

    //-------------------------------------------------------------------------
    // LanguagePrimitives.HashCompare: Comparison
    //
    // Bi-modal generic comparison helper implementation.
    //
    // The comparison implementation is run in either Equivalence Relation or Partial 
    // Equivalence Relation (PER) mode which governs what happens when NaNs are compared.
    //
    // Some representations chosen by F# are legitimately allowed to be null, e.g. the None value.
    // However, null values don't support the polymorphic virtual comparison operation CompareTo 
    // so the test for nullness must be made on the caller side.
    //------------------------------------------------------------------------- 

    let FailGenericComparison (obj: obj)  = 
        failwith "raise (new System.ArgumentException(String.Format(SR.GetString(SR.genericCompareFail1), obj.GetType().ToString())))"
       
    /// This type has two instances - fsComparerER and fsComparerThrow.
    ///   - fsComparerER  = ER semantics = no throw on NaN comparison = new GenericComparer(false) = GenericComparer = GenericComparison
    ///   - fsComparerPER  = PER semantics = local throw on NaN comparison = new GenericComparer(true) = LessThan/GreaterThan etc.
    type GenericComparer(throwsOnPER:bool) = 
        interface System.Collections.IComparer 
        member  c.ThrowsOnPER = throwsOnPER

    /// The unique exception object that is thrown locally when NaNs are compared in PER mode (by fsComparerPER)
    /// This exception should never be observed by user code.
    let NaNException = new System.Exception()                                                 
            
    let inline ArrayComparison<'T> (f:'T->'T->int) (x:'T[]) (y:'T[]) : int =
        let lenx = x.Length 
        let leny = y.Length 
        let rec loop c i =
            if c <> 0 then Math.Sign c
            elif i = lenx then 0
            else loop (f (x.[i]) (y.[i])) (i+1)
        loop (lenx-leny) 0

    let GenericComparisonByteArray (x:byte[]) (y:byte[]) : int = ArrayComparison (fun x y -> (# "conv.i4" x : int32 #)-(# "conv.i4" y : int32 #)) x y

    /// Implements generic comparison between two objects. This corresponds to the pseudo-code in the F#
    /// specification.  The treatment of NaNs is governed by "comp".
    let rec GenericCompare (comp:GenericComparer) (xobj:obj,yobj:obj) =
        (*if objEq xobj yobj then 0 else *)
          match xobj,yobj with 
           | null,null -> 0
           | null,_ -> -1
           | _,null -> 1
           // Use Ordinal comparison for strings
           | (:? string as x),(:? string as y) -> System.String.CompareOrdinal(x, y)
           // Permit structural comparison on arrays
           | (:? System.Array as arr1),_ -> 
               match arr1,yobj with 
               | (:? (obj[]) as arr1), (:? (obj[]) as arr2)-> GenericComparisonObjArrayWithComparer comp arr1 arr2

               // The additional equality check is required here because .net treats byte[] and sbyte[] as cast-compatible
               // (but comparison is different)
               | (:? (byte[]) as arr1), (:? (byte[]) as arr2) 
                    when typeof<byte[]>.Equals (arr1.GetType ()) && typeof<byte[]>.Equals (arr2.GetType ()) -> GenericComparisonByteArray arr1 arr2

               | _                   , (:? System.Array as arr2) -> GenericComparisonArbArrayWithComparer comp arr1 arr2
               | _ -> FailGenericComparison xobj
           // Check for IStructuralComparable
           | (:? IStructuralComparable as x),_ ->
               x.CompareTo(yobj,comp)
           // Check for IComparable
           | (:? System.IComparable as x),_ -> 
               if comp.ThrowsOnPER then 
                   match xobj,yobj with 
                   | (:? float as x),(:? float as y) -> 
                        if (System.Double.IsNaN x || System.Double.IsNaN y) then 
                            raise NaNException
                   | (:? float32 as x),(:? float32 as y) -> 
                        if (System.Single.IsNaN x || System.Single.IsNaN y) then 
                            raise NaNException
                   | _ -> ()
               x.CompareTo(yobj)
           | (:? nativeint as x),(:? nativeint as y) -> if (# "clt" x y : bool #) then (-1) else (# "cgt" x y : int #)
           | (:? unativeint as x),(:? unativeint as y) -> if (# "clt.un" x y : bool #) then (-1) else (# "cgt.un" x y : int #)
           | _,(:? IStructuralComparable as yc) ->
               let res = yc.CompareTo(xobj,comp)
               if res < 0 then 1 elif res > 0 then -1 else 0
           | _,(:? System.IComparable as yc) -> 
               // Note -c doesn't work here: be careful of comparison function returning minint
               let c = yc.CompareTo(xobj) in 
               if c < 0 then 1 elif c > 0 then -1 else 0
           | _ -> FailGenericComparison xobj

    /// specialcase: Core implementation of structural comparison on arbitrary arrays.
    and GenericComparisonArbArrayWithComparer (comp:GenericComparer) (x:System.Array) (y:System.Array) : int  =
#if FX_NO_ARRAY_LONG_LENGTH            
        if x.Rank = 1 && y.Rank = 1 then 
            let lenx = x.Length
            let leny = y.Length 
            let c = intOrder lenx leny 
            if c <> 0 then c else
            let basex = (x.GetLowerBound(0))
            let basey = (y.GetLowerBound(0))
            let c = intOrder basex basey
            if c <> 0 then c else
            let rec check i =
               if i >= lenx then 0 else 
               let c = GenericCompare comp ((x.GetValue(i + basex)),(y.GetValue(i + basey)))
               if c <> 0 then c else check (i + 1)
            check 0
        elif x.Rank = 2 && y.Rank = 2 then 
            let lenx0 = x.GetLength(0)
            let leny0 = y.GetLength(0)
            let c = intOrder lenx0 leny0 
            if c <> 0 then c else
            let lenx1 = x.GetLength(1)
            let leny1 = y.GetLength(1)
            let c = intOrder lenx1 leny1 
            if c <> 0 then c else
            let basex0 = (x.GetLowerBound(0))
            let basex1 = (x.GetLowerBound(1))
            let basey0 = (y.GetLowerBound(0))
            let basey1 = (y.GetLowerBound(1))
            let c = intOrder basex0 basey0
            if c <> 0 then c else
            let c = intOrder basex1 basey1
            if c <> 0 then c else
            let rec check0 i =
               let rec check1 j = 
                   if j >= lenx1 then 0 else
                   let c = GenericCompare comp ((x.GetValue(i + basex0,j + basex1)), (y.GetValue(i + basey0,j + basey1)))
                   if c <> 0 then c else check1 (j + 1)
               if i >= lenx0 then 0 else 
               let c = check1 0
               if c <> 0 then c else
               check0 (i + 1)
            check0 0
        else
            let c = intOrder x.Rank y.Rank
            if c <> 0 then c else
            let ndims = x.Rank
            // check lengths 
            let rec precheck k = 
                if k >= ndims then 0 else
                let c = intOrder (x.GetLength(k)) (y.GetLength(k))
                if c <> 0 then c else
                let c = intOrder (x.GetLowerBound(k)) (y.GetLowerBound(k))
                if c <> 0 then c else
                precheck (k+1)
            let c = precheck 0 
            if c <> 0 then c else
            let idxs : int[] = zeroCreate ndims 
            let rec checkN k baseIdx i lim =
               if i >= lim then 0 else
               set idxs k (baseIdx + i)
               let c = 
                   if k = ndims - 1
                   then GenericCompare comp ((x.GetValue(idxs)), (y.GetValue(idxs)))
                   else check (k+1) 
               if c <> 0 then c else 
               checkN k baseIdx (i + 1) lim
            and check k =
               if k >= ndims then 0 else
               let baseIdx = x.GetLowerBound(k)
               checkN k baseIdx 0 (x.GetLength(k))
            check 0
#else
        if x.Rank = 1 && y.Rank = 1 then 
            let lenx = x.LongLength
            let leny = y.LongLength 
            let c = int64Order lenx leny 
            if c <> 0 then c else
            let basex = int64 (x.GetLowerBound(0))
            let basey = int64 (y.GetLowerBound(0))
            let c = int64Order basex basey
            if c <> 0 then c else
            let rec check i =
               if i >=. lenx then 0 else 
               let c = GenericCompare comp ((x.GetValue(i +. basex)), (y.GetValue(i +. basey)))
               if c <> 0 then c else check (i +. 1L)
            check 0L
        elif x.Rank = 2 && y.Rank = 2 then 
            let lenx0 = x.GetLongLength(0)
            let leny0 = y.GetLongLength(0)
            let c = int64Order lenx0 leny0 
            if c <> 0 then c else
            let lenx1 = x.GetLongLength(1)
            let leny1 = y.GetLongLength(1)
            let c = int64Order lenx1 leny1 
            if c <> 0 then c else
            let basex0 = int64 (x.GetLowerBound(0))
            let basey0 = int64 (y.GetLowerBound(0))
            let c = int64Order basex0 basey0
            if c <> 0 then c else
            let basex1 = int64 (x.GetLowerBound(1))
            let basey1 = int64 (y.GetLowerBound(1))
            let c = int64Order basex1 basey1
            if c <> 0 then c else
            let rec check0 i =
               let rec check1 j = 
                   if j >=. lenx1 then 0 else
                   let c = GenericCompare comp ((x.GetValue(i +. basex0,j +. basex1)), (y.GetValue(i +. basey0,j +. basey1)))
                   if c <> 0 then c else check1 (j +. 1L)
               if i >=. lenx0 then 0 else 
               let c = check1 0L
               if c <> 0 then c else
               check0 (i +. 1L)
            check0 0L
        else
            let c = intOrder x.Rank y.Rank
            if c <> 0 then c else
            let ndims = x.Rank
            // check lengths 
            let rec precheck k = 
                if k >= ndims then 0 else
                let c = int64Order (x.GetLongLength(k)) (y.GetLongLength(k))
                if c <> 0 then c else
                let c = intOrder (x.GetLowerBound(k)) (y.GetLowerBound(k))
                if c <> 0 then c else
                precheck (k+1)
            let c = precheck 0 
            if c <> 0 then c else
            let idxs : int64[] = zeroCreate ndims 
            let rec checkN k baseIdx i lim =
               if i >=. lim then 0 else
               set idxs k (baseIdx +. i)
               let c = 
                   if k = ndims - 1
                   then GenericCompare comp ((x.GetValue(idxs)), (y.GetValue(idxs)))
                   else check (k+1) 
               if c <> 0 then c else 
               checkN k baseIdx (i +. 1L) lim
            and check k =
               if k >= ndims then 0 else
               let baseIdx = x.GetLowerBound(k)
               checkN k (int64 baseIdx) 0L (x.GetLongLength(k))
            check 0
#endif                
      
    and GenericComparisonObjArrayWithComparer (comp:GenericComparer) (x:obj[]) (y:obj[]) : int =
        ArrayComparison (fun x y -> GenericCompare comp (x, y)) x y

    type GenericComparer with
        interface System.Collections.IComparer with
            override c.Compare(x:obj,y:obj) = GenericCompare c (x,y)
    
    /// The unique object for comparing values in PER mode (where local exceptions are thrown when NaNs are compared)
    let fsComparerPER = GenericComparer(true)  

    /// The unique object for comparing values in ER mode (where "0" is returned when NaNs are compared)
    let fsComparerER = GenericComparer(false) 

    let isStructuralComparable (ty:Type) = typeof<IStructuralComparable>.IsAssignableFrom ty
    let isValueTypeStructuralComparable (ty:Type) = isStructuralComparable ty && ty.IsValueType

    let canUseDefaultComparer er (rootType:Type) = 
        // "Default" equality for strings is culturally sensitive, so needs special handling
        canUseDotnetDefaultComparisonOrEquality Reflection.hasCustomComparison isStructuralComparable true er rootType

    type ComparisonUsage = 
    | ERUsage           = 0
    | PERUsage          = 1
    | LessThanUsage     = 2
    | GreaterThanUsage  = 3

    [<Literal>]
    let LessThanUsageReturnFalse = 1
    [<Literal>]
    let GreaterThanUsageReturnFalse = -1

    let inline signedComparer<'T> () =
        box { new Comparer<'T>() with
                member __.Compare (x,y) =
                    if (# "clt" x y : bool #) then -1
                    else (# "cgt" x y : int #) }

    let inline unsignedComparer<'T> () =
        box { new Comparer<'T>() with
                member __.Compare (x,y) =
                    if (# "clt.un" x y : bool #) then -1
                    else (# "cgt.un" x y : int #) }

    let inline floatingPointComparer<'T> onNaN =
        box { new Comparer<'T>() with
                member __.Compare (x,y) =
                    if   (# "clt" x y : bool #) then -1
                    elif (# "cgt" x y : bool #) then 1
                    elif (# "ceq" x y : bool #) then 0
                    else onNaN () }

    let tryGetFSharpComparer (usage:ComparisonUsage) (externalUse:bool) (ty:Type) : obj =
        match usage, externalUse, ty with
        | ComparisonUsage.ERUsage, _, ty when ty.Equals typeof<float>   -> box Comparer<float>.Default
        | ComparisonUsage.ERUsage, _, ty when ty.Equals typeof<float32> -> box Comparer<float32>.Default

        | ComparisonUsage.PERUsage,         _, ty when ty.Equals typeof<float> -> floatingPointComparer<float> (fun () -> raise NaNException)
        | ComparisonUsage.LessThanUsage,    _, ty when ty.Equals typeof<float> -> floatingPointComparer<float> (fun () -> LessThanUsageReturnFalse)
        | ComparisonUsage.GreaterThanUsage, _, ty when ty.Equals typeof<float> -> floatingPointComparer<float> (fun () -> GreaterThanUsageReturnFalse)

        | ComparisonUsage.PERUsage,         _, ty when ty.Equals typeof<float32> -> floatingPointComparer<float32> (fun () -> raise NaNException)
        | ComparisonUsage.LessThanUsage,    _, ty when ty.Equals typeof<float32> -> floatingPointComparer<float32> (fun () -> LessThanUsageReturnFalse)
        | ComparisonUsage.GreaterThanUsage, _, ty when ty.Equals typeof<float32> -> floatingPointComparer<float32> (fun () -> GreaterThanUsageReturnFalse)

        // the implemention of Comparer<string>.Default returns a current culture specific comparer
        | _, _, ty when ty.Equals typeof<string> ->
            box { new Comparer<string>() with
                    member __.Compare (x,y) =
                        System.String.CompareOrdinal (x, y) }

        | _, _, ty when ty.Equals typeof<unativeint> -> unsignedComparer<unativeint> ()
        | _, _, ty when ty.Equals typeof<nativeint>  -> signedComparer<nativeint> ()

        // these are used as external facing comparers for compatability (they always return -1/0/+1)
        | _, true, ty when ty.Equals typeof<byte>   -> unsignedComparer<byte> ()
        | _, true, ty when ty.Equals typeof<sbyte>  -> signedComparer<sbyte> ()
        | _, true, ty when ty.Equals typeof<int16>  -> signedComparer<int16> ()
        | _, true, ty when ty.Equals typeof<uint16> -> unsignedComparer<uint16> ()
        | _, true, ty when ty.Equals typeof<char>   -> unsignedComparer<char> ()
        
        | _ -> null

    let inline nullableComparer<'a when 'a : null> compare = 
        box { new Comparer<'a>() with
                member __.Compare (x,y)  =
                    match x, y with
                    | null, null -> 0
                    | null, _ -> -1
                    | _, null -> 1
                    | _ -> compare x y }

    let tryGetFSharpArrayComparer (ty:Type) comparer : obj =
        if   ty.Equals typeof<obj[]>  then nullableComparer (fun x y -> GenericComparisonObjArrayWithComparer comparer x y)
        elif ty.Equals typeof<byte[]> then nullableComparer GenericComparisonByteArray
        else null

    let arrayComparer<'T> comparer =
        match tryGetFSharpArrayComparer typeof<'T> comparer with
        | :? Comparer<'T> as arrayComparer -> arrayComparer
        | _ -> 
            { new Comparer<'T>() with
                member __.Compare (x, y) =
                    match box x, box y with 
                    | null, null -> 0
                    | null, _    -> -1
                    | _,    null -> 1
                    | (:? System.Array as arr1), (:? System.Array as arr2) -> GenericComparisonArbArrayWithComparer comparer arr1 arr2
                    | _ -> raise (Exception "invalid logic - expected System.Array")  }

    let structuralComparer<'T> comparer =
        { new Comparer<'T>() with
            member __.Compare (x,y) =
                match box x, box y with 
                | null, null -> 0
                | null, _    -> -1
                | _,    null -> 1
                | (:? IStructuralComparable as x1), yobj -> x1.CompareTo (yobj, comparer)
                | _ -> raise (Exception "invalid logic - expected IStructuralEquatable") }

    let structuralComparerValueType<'T> comparer =
        { new Comparer<'T>() with
            member __.Compare (x,y) =
                ((box x):?>IStructuralComparable).CompareTo (y, comparer) }

    let unknownComparer<'T> comparer =
        { new Comparer<'T>() with
            member __.Compare (x,y) =
                GenericCompare comparer (box x, box y) }

    // this wrapper is used with the comparison operators to cause a false result when a NaNException
    // has been thrown somewhere in the tested objects hierarchy
    let maybeNaNExceptionComparer<'T> (comparer:Comparer<'T>) valueToCauseFalse =
        { new Comparer<'T>() with
            member __.Compare (x,y) =
                try
                    comparer.Compare (x,y)
                with
                    e when System.Runtime.CompilerServices.RuntimeHelpers.Equals(e, NaNException) -> valueToCauseFalse }

    let getGenericComparison<'T> usage externalUse =
        let er = match usage with ComparisonUsage.ERUsage -> true | _ -> false

        match tryGetFSharpComparer usage externalUse typeof<'T> with
        | :? Comparer<'T> as comparer                -> comparer
        | _ when canUseDefaultComparer er typeof<'T> -> Comparer<'T>.Default
        | _ ->
            if er then
                if   isArray typeof<'T>                         then arrayComparer               fsComparerER
                elif isValueTypeStructuralComparable typeof<'T> then structuralComparerValueType fsComparerER
                elif isStructuralComparable typeof<'T>          then structuralComparer          fsComparerER
                else                                                 unknownComparer             fsComparerER
            else
                let comparer  =
                    if   isArray typeof<'T>                         then arrayComparer               fsComparerPER
                    elif isValueTypeStructuralComparable typeof<'T> then structuralComparerValueType fsComparerPER
                    elif isStructuralComparable typeof<'T>          then structuralComparer          fsComparerPER
                    else                                                 unknownComparer             fsComparerPER

                match usage with
                | ComparisonUsage.LessThanUsage    -> maybeNaNExceptionComparer comparer LessThanUsageReturnFalse
                | ComparisonUsage.GreaterThanUsage -> maybeNaNExceptionComparer comparer GreaterThanUsageReturnFalse
                | _ -> comparer

    /// As an optimization, determine if a fast unstable sort can be used with equivalent results
    let equivalentForStableAndUnstableSort (ty:Type) =
        ty.Equals(typeof<byte>)      
        || ty.Equals(typeof<char>)      
        || ty.Equals(typeof<sbyte>)      
        || ty.Equals(typeof<int16>)     
        || ty.Equals(typeof<int32>)     
        || ty.Equals(typeof<int64>)     
        || ty.Equals(typeof<uint16>)    
        || ty.Equals(typeof<uint32>)    
        || ty.Equals(typeof<uint64>)    
        || ty.Equals(typeof<float>)     
        || ty.Equals(typeof<float32>)   
        || ty.Equals(typeof<decimal>)   

    [<AbstractClass; Sealed>]
    type FSharpComparer_ER<'T> private () =
        static let comparer = getGenericComparison<'T> ComparisonUsage.ERUsage true
        static member Comparer = comparer

    [<AbstractClass; Sealed>]
    type FSharpComparer_InternalUse_ER<'T> private () =
        static let equivalentForStableAndUnstableSort = equivalentForStableAndUnstableSort typeof<'T>
        static let comparer = getGenericComparison<'T> ComparisonUsage.ERUsage false
        static member Comparer = comparer
        static member EquivalentForStableAndUnstableSort = equivalentForStableAndUnstableSort

    [<AbstractClass; Sealed>]
    type FSharpComparer_PER<'T> private () =
        static let comparer = getGenericComparison<'T> ComparisonUsage.PERUsage false
        static member Comparer = comparer

    [<AbstractClass; Sealed>]
    type FSharpComparer_ForLessThanComparison<'T> private () =
        static let comparer = getGenericComparison<'T> ComparisonUsage.LessThanUsage false
        static member Comparer = comparer

    [<AbstractClass; Sealed>]
    type FSharpComparer_ForGreaterThanComparison<'T> private () =
        static let comparer = getGenericComparison<'T> ComparisonUsage.GreaterThanUsage false
        static member Comparer = comparer

    /// Compare two values of the same generic type, using "comp".
    //
    // "comp" is assumed to be either fsComparerPER or fsComparerER (and hence 'Compare' is implemented via 'GenericCompare').
    //
    // NOTE: the compiler optimizer is aware of this function and devirtualizes in the 
    // cases where it is known how a particular type implements generic comparison.
    let GenericComparisonWithComparerIntrinsic<'T> (comp:System.Collections.IComparer) (x:'T) (y:'T) : int = 
        if obj.ReferenceEquals (comp, fsComparerER) then
            FSharpComparer_InternalUse_ER.Comparer.Compare (x, y)
        elif obj.ReferenceEquals (comp, fsComparerPER) then
            FSharpComparer_PER.Comparer.Compare (x, y)
        else
            comp.Compare (box x, box y)


    /// Generic comparison. Implements ER mode (where "0" is returned when NaNs are compared)
    //
    // The compiler optimizer is aware of this function  (see use of generic_comparison_inner_vref in opt.fs)
    // and devirtualizes calls to it based on "T".
    let GenericComparisonIntrinsic<'T> (x:'T) (y:'T) : int = 
        FSharpComparer_ER.Comparer.Compare (x, y)

    /// Generic less-than.
    let GenericLessThanIntrinsic (x:'T) (y:'T) = 
        (# "clt" (FSharpComparer_ForLessThanComparison.Comparer.Compare (x, y)) 0 : bool #)
    
    /// Generic greater-than.
    let GenericGreaterThanIntrinsic (x:'T) (y:'T) = 
        (# "cgt" (FSharpComparer_ForGreaterThanComparison.Comparer.Compare (x, y)) 0 : bool #)
     
    /// Generic greater-than-or-equal.
    let GenericGreaterOrEqualIntrinsic (x:'T) (y:'T) = 
        (# "cgt" (FSharpComparer_ForGreaterThanComparison.Comparer.Compare (x, y)) -1 : bool #)
    
    /// Generic less-than-or-equal.
    let GenericLessOrEqualIntrinsic (x:'T) (y:'T) = 
        (# "clt" (FSharpComparer_ForLessThanComparison.Comparer.Compare (x, y)) 1 : bool #)


    //-------------------------------------------------------------------------
    // LanguagePrimitives.HashCompare: EQUALITY
    //------------------------------------------------------------------------- 

    let inline ArrayEquality<'T> (f:'T->'T->bool) (x:'T[]) (y:'T[])  : bool =
        let lenx = x.Length 
        let leny = y.Length 
        let  rec loop i =
            if i = lenx then true
            elif f (get x i) (get y i) then loop (i+1)
            else false
        (lenx = leny) && loop 0

    let inline ArrayEqualityWithERFlag<'T> (er:bool) (f:'T->'T->bool) (x:'T[]) (y:'T[]) : bool =
        if er
        then ArrayEquality (fun x y -> (not (f x x) && not (f y y)) || (f x y)) x y
        else ArrayEquality f x y

    let GenericEqualityByteArray      x y = ArrayEquality              byteEq    x y
    let GenericEqualityInt32Array     x y = ArrayEquality              int32Eq   x y
    let GenericEqualitySingleArray er x y = ArrayEqualityWithERFlag er float32Eq x y
    let GenericEqualityDoubleArray er x y = ArrayEqualityWithERFlag er floatEq   x y
    let GenericEqualityCharArray      x y = ArrayEquality              charEq    x y
    let GenericEqualityInt64Array     x y = ArrayEquality              int64Eq   x y

    /// The core implementation of generic equality between two objects.  This corresponds
    /// to th e pseudo-code in the F# language spec.
    //
    // Run in either PER or ER mode.  In PER mode, equality involving a NaN returns "false".
    // In ER mode, equality on two NaNs returns "true".
    //
    // If "er" is true the "iec" is fsEqualityComparerUnlimitedHashingER
    // If "er" is false the "iec" is fsEqualityComparerUnlimitedHashingPER
    let rec GenericEqualityObj (er:bool) (iec:System.Collections.IEqualityComparer) ((xobj:obj),(yobj:obj)) : bool = 
        (*if objEq xobj yobj then true else  *)
        match xobj,yobj with 
         | null,null -> true
         | null,_ -> false
         | _,null -> false
         | (:? string as xs),(:? string as ys) -> System.String.Equals(xs,ys)
         // Permit structural equality on arrays
         | (:? System.Array as arr1),_ -> 
             // due to the rules of the CLI type system, array casts are "assignment compatible"
             // see: https://blogs.msdn.microsoft.com/ericlippert/2009/09/24/why-is-covariance-of-value-typed-arrays-inconsistent/
             // this means that the cast and comparison for byte will also handle sbyte, int32 handle uint32, 
             // and int64 handle uint64. Equality will still be correct.
             match arr1,yobj with 
             | (:? (obj[]) as arr1),    (:? (obj[]) as arr2)      -> GenericEqualityObjArray er iec arr1 arr2
             | (:? (byte[]) as arr1),    (:? (byte[]) as arr2)     -> GenericEqualityByteArray arr1 arr2
             | (:? (int32[]) as arr1),   (:? (int32[]) as arr2)   -> GenericEqualityInt32Array arr1 arr2
             | (:? (int64[]) as arr1),   (:? (int64[]) as arr2)   -> GenericEqualityInt64Array arr1 arr2
             | (:? (char[]) as arr1),    (:? (char[]) as arr2)     -> GenericEqualityCharArray arr1 arr2
             | (:? (float32[]) as arr1), (:? (float32[]) as arr2) -> GenericEqualitySingleArray er arr1 arr2
             | (:? (float[]) as arr1),   (:? (float[]) as arr2)     -> GenericEqualityDoubleArray er arr1 arr2
             | _                   ,    (:? System.Array as arr2) -> GenericEqualityArbArray er iec arr1 arr2
             | _ -> xobj.Equals(yobj)
         | (:? IStructuralEquatable as x1),_ -> x1.Equals(yobj,iec)
         // Ensure ER NaN semantics on recursive calls
         | (:? float as f1), (:? float as f2) ->
            if er && (not (# "ceq" f1 f1 : bool #)) && (not (# "ceq" f2 f2 : bool #)) then true // NAN with ER semantics
            else (# "ceq" f1 f2 : bool #) // PER semantics
         | (:? float32 as f1), (:? float32 as f2) ->
            if er && (not (# "ceq" f1 f1 : bool #)) && (not (# "ceq" f2 f2 : bool #)) then true // NAN with ER semantics
            else (# "ceq" f1 f2 : bool #)  // PER semantics
         | _ -> xobj.Equals(yobj)

    /// specialcase: Core implementation of structural equality on arbitrary arrays.
    and GenericEqualityArbArray er (iec:System.Collections.IEqualityComparer) (x:System.Array) (y:System.Array) : bool =
#if FX_NO_ARRAY_LONG_LENGTH
        if x.Rank = 1 && y.Rank = 1 then 
            // check lengths 
            let lenx = x.Length
            let leny = y.Length 
            (int32Eq lenx leny) &&
            // check contents
            let basex = x.GetLowerBound(0)
            let basey = y.GetLowerBound(0)
            (int32Eq basex basey) &&
            let rec check i = (i >= lenx) || (GenericEqualityObj er iec ((x.GetValue(basex + i)),(y.GetValue(basey + i))) && check (i + 1))
            check 0                    
        elif x.Rank = 2 && y.Rank = 2 then 
            // check lengths 
            let lenx0 = x.GetLength(0)
            let leny0 = y.GetLength(0)
            (int32Eq lenx0 leny0) && 
            let lenx1 = x.GetLength(1)
            let leny1 = y.GetLength(1)
            (int32Eq lenx1 leny1) && 
            let basex0 = x.GetLowerBound(0)
            let basex1 = x.GetLowerBound(1)
            let basey0 = y.GetLowerBound(0)
            let basey1 = y.GetLowerBound(1)
            (int32Eq basex0 basey0) && 
            (int32Eq basex1 basey1) && 
            // check contents
            let rec check0 i =
               let rec check1 j = (j >= lenx1) || (GenericEqualityObj er iec ((x.GetValue(basex0 + i,basex1 + j)), (y.GetValue(basey0 + i,basey1 + j))) && check1 (j + 1))
               (i >= lenx0) || (check1 0 && check0 (i + 1))
            check0 0
        else 
            (x.Rank = y.Rank) && 
            let ndims = x.Rank
            // check lengths 
            let rec precheck k = 
                (k >= ndims) || 
                (int32Eq (x.GetLength(k)) (y.GetLength(k)) && 
                 int32Eq (x.GetLowerBound(k)) (y.GetLowerBound(k)) && 
                 precheck (k+1))
            precheck 0 &&
            let idxs : int32[] = zeroCreate ndims 
            // check contents
            let rec checkN k baseIdx i lim =
               (i >= lim) ||
               (set idxs k (baseIdx + i);
                (if k = ndims - 1 
                 then GenericEqualityObj er iec ((x.GetValue(idxs)),(y.GetValue(idxs)))
                 else check (k+1)) && 
                checkN k baseIdx (i + 1) lim)
            and check k = 
               (k >= ndims) || 
               (let baseIdx = x.GetLowerBound(k)
                checkN k baseIdx 0 (x.GetLength(k)))
                   
            check 0
#else
        if x.Rank = 1 && y.Rank = 1 then 
            // check lengths 
            let lenx = x.LongLength
            let leny = y.LongLength 
            (int64Eq lenx leny) &&
            // check contents
            let basex = int64 (x.GetLowerBound(0))
            let basey = int64 (y.GetLowerBound(0))
            (int64Eq basex basey) &&                    
            let rec check i = (i >=. lenx) || (GenericEqualityObj er iec ((x.GetValue(basex +. i)),(y.GetValue(basey +. i))) && check (i +. 1L))
            check 0L                    
        elif x.Rank = 2 && y.Rank = 2 then 
            // check lengths 
            let lenx0 = x.GetLongLength(0)
            let leny0 = y.GetLongLength(0)
            (int64Eq lenx0 leny0) && 
            let lenx1 = x.GetLongLength(1)
            let leny1 = y.GetLongLength(1)
            (int64Eq lenx1 leny1) && 
            let basex0 = int64 (x.GetLowerBound(0))
            let basex1 = int64 (x.GetLowerBound(1))
            let basey0 = int64 (y.GetLowerBound(0))
            let basey1 = int64 (y.GetLowerBound(1))
            (int64Eq basex0 basey0) && 
            (int64Eq basex1 basey1) && 
            // check contents
            let rec check0 i =
               let rec check1 j = (j >=. lenx1) || (GenericEqualityObj er iec ((x.GetValue(basex0 +. i,basex1 +. j)),(y.GetValue(basey0 +. i,basey1 +. j))) && check1 (j +. 1L))
               (i >=. lenx0) || (check1 0L && check0 (i +. 1L))
            check0 0L
        else 
            (x.Rank = y.Rank) && 
            let ndims = x.Rank
            // check lengths 
            let rec precheck k = 
                (k >= ndims) || 
                (int64Eq (x.GetLongLength(k)) (y.GetLongLength(k)) && 
                 int32Eq (x.GetLowerBound(k)) (y.GetLowerBound(k)) && 
                 precheck (k+1))
            precheck 0 &&
            let idxs : int64[] = zeroCreate ndims 
            // check contents
            let rec checkN k baseIdx i lim =
               (i >=. lim) ||
               (set idxs k (baseIdx +. i);
                (if k = ndims - 1
                 then GenericEqualityObj er iec ((x.GetValue(idxs)),(y.GetValue(idxs)))
                 else check (k+1)) && 
                checkN k baseIdx (i +. 1L) lim)
            and check k = 
               (k >= ndims) || 
               (let baseIdx = x.GetLowerBound(k)
                checkN k (int64 baseIdx) 0L (x.GetLongLength(k)))
                   
            check 0
#endif                    
      
    /// optimized case: Core implementation of structural equality on object arrays.
    and GenericEqualityObjArray er iec (xarray:obj[]) (yarray:obj[]) : bool =
        ArrayEquality (fun x y -> GenericEqualityObj er iec (x, y)) xarray yarray

    let isStructuralEquatable (ty:Type) = typeof<IStructuralEquatable>.IsAssignableFrom ty
    let isValueTypeStructuralEquatable (ty:Type) = isStructuralEquatable ty && ty.IsValueType

    let canUseDefaultEqualityComparer er (rootType:Type) =
        // "Default" equality for strings is by ordinal, so needs special handling required
        canUseDotnetDefaultComparisonOrEquality Reflection.hasCustomEquality isStructuralEquatable false er rootType

    let tryGetFSharpEqualityComparer (er:bool) (ty:Type) : obj =
        match er, ty with
        | false, ty when ty.Equals typeof<float> ->
            box { new EqualityComparer<float>() with
                    member __.Equals (x,y) = (# "ceq" x y : bool #)
                    member __.GetHashCode x = x.GetHashCode () }
        | false, ty when ty.Equals typeof<float32> ->
            box { new EqualityComparer<float32>() with
                    member __.Equals (x,y) = (# "ceq" x y : bool #)
                    member __.GetHashCode x = x.GetHashCode () }
        | true, ty when ty.Equals typeof<float>   -> box EqualityComparer<float>.Default
        | true, ty when ty.Equals typeof<float32> -> box EqualityComparer<float32>.Default
        | _ -> null

    let inline nullableEqualityComparer<'a when 'a : null> equals getHashCode = 
        box { new EqualityComparer<'a>() with
                member __.Equals (x,y)  =
                    match x, y with
                    | null, null -> true
                    | null, _ -> false
                    | _, null -> false
                    | _ -> equals x y

                member __.GetHashCode x =
                    match x with
                    | null -> 0
                    | _ -> getHashCode x }

    let inline castNullableEqualityComparer<'fromType, 'toType when 'toType : null and 'fromType : null> (equals:'toType->'toType->bool) (getHashCode:'toType->int) =
        let castEquals (lhs:'fromType) (rhs:'fromType) = equals (unbox lhs) (unbox rhs)
        let castGetHashCode (o:'fromType) = getHashCode (unbox o)
        nullableEqualityComparer castEquals castGetHashCode

    let tryGetFSharpArrayEqualityComparer (ty:Type) er comparer : obj =
        // the casts here between byte+sbyte, int32+uint32 and int64+uint64 are here to replicate the behaviour
        // in GenericHashParamObj
        if   ty.Equals typeof<obj[]>    then nullableEqualityComparer (fun x y -> GenericEqualityObjArray er comparer x y) (GenericHashObjArray fsEqualityComparerUnlimitedHashingPER)
        elif ty.Equals typeof<byte[]>   then nullableEqualityComparer                 GenericEqualityByteArray              GenericHashByteArray
        elif ty.Equals typeof<sbyte[]>  then castNullableEqualityComparer<sbyte[],_>  GenericEqualityByteArray              GenericHashByteArray
        elif ty.Equals typeof<int32[]>  then nullableEqualityComparer                 GenericEqualityInt32Array             GenericHashInt32Array
        elif ty.Equals typeof<uint32[]> then castNullableEqualityComparer<uint32[],_> GenericEqualityInt32Array             GenericHashInt32Array
        elif ty.Equals typeof<int64[]>  then nullableEqualityComparer                 GenericEqualityInt64Array             GenericHashInt64Array
        elif ty.Equals typeof<uint64[]> then castNullableEqualityComparer<uint64[],_> GenericEqualityInt64Array             GenericHashInt64Array
        else null

    let arrayEqualityComparer<'T> er comparer =
        match tryGetFSharpArrayEqualityComparer typeof<'T> er comparer with
        | :? EqualityComparer<'T> as arrayComparer -> arrayComparer
        | _ -> 
            { new EqualityComparer<'T>() with
                member __.Equals (x, y) =
                    let xobj, yobj = box x, box y
                    match xobj,yobj with 
                    | null, null -> true
                    | null, _ -> false
                    | _, null -> false
                    | (:? (char[]) as arr1), (:? (char[]) as arr2) -> GenericEqualityCharArray arr1 arr2
                    | _ ->
                    match xobj,yobj with 
                    | (:? (float32[]) as arr1), (:? (float32[]) as arr2) -> GenericEqualitySingleArray er arr1 arr2
                    | _ ->
                    match xobj,yobj with 
                    | (:? (float[]) as arr1), (:? (float[])as arr2) -> GenericEqualityDoubleArray er arr1 arr2
                    | _ ->
                    match xobj,yobj with 
                    | (:? System.Array as arr1), (:? System.Array as arr2) -> GenericEqualityArbArray er comparer arr1 arr2
                    | _ -> raise (Exception "invalid logic - expected array")

                member __.GetHashCode x = 
                    match box x with 
                    | null -> 0 
                    | :? System.Array as a -> GenericHashArbArray fsEqualityComparerUnlimitedHashingPER a 
                    | _ -> raise (Exception "invalid logic - expected array") }

    let structuralEqualityComparer<'T> comparer =
        { new EqualityComparer<'T>() with
            member __.Equals (x,y) =
                match box x, box y with 
                | null, null -> true
                | null, _    -> false
                | _,    null -> false
                | (:? IStructuralEquatable as x1), yobj -> x1.Equals (yobj, comparer)
                | _ -> raise (Exception "invalid logic - expected IStructuralEquatable")

            member __.GetHashCode x =
                match box x with 
                | null -> 0 
                | :? IStructuralEquatable as a -> a.GetHashCode fsEqualityComparerUnlimitedHashingPER
                | _ -> raise (Exception "invalid logic - expected IStructuralEquatable") }

    let structuralEqualityComparerValueType<'T> comparer =
        { new EqualityComparer<'T>() with
            member __.Equals (x,y)  = ((box x):?>IStructuralEquatable).Equals (y, comparer)
            member __.GetHashCode x = ((box x):?>IStructuralEquatable).GetHashCode fsEqualityComparerUnlimitedHashingPER }

    let unknownEqualityComparer<'T> er comparer =
        { new EqualityComparer<'T>() with
            member __.Equals (x,y) = GenericEqualityObj er comparer (box x, box y)
            member __.GetHashCode x = GenericHashParamObj fsEqualityComparerUnlimitedHashingPER (box x) }

    let getGenericEquality<'T> er =
        match tryGetFSharpEqualityComparer er typeof<'T> with
        | :? EqualityComparer<'T> as call                        -> call
        | _ when canUseDefaultEqualityComparer er typeof<'T>     -> EqualityComparer<'T>.Default
        | _ when isArray typeof<'T> && er                        -> arrayEqualityComparer true  fsEqualityComparerUnlimitedHashingER
        | _ when isArray typeof<'T>                              -> arrayEqualityComparer false fsEqualityComparerUnlimitedHashingPER
        | _ when isValueTypeStructuralEquatable typeof<'T> && er -> structuralEqualityComparerValueType fsEqualityComparerUnlimitedHashingER
        | _ when isValueTypeStructuralEquatable typeof<'T>       -> structuralEqualityComparerValueType fsEqualityComparerUnlimitedHashingPER
        | _ when isStructuralEquatable typeof<'T> && er          -> structuralEqualityComparer fsEqualityComparerUnlimitedHashingER
        | _ when isStructuralEquatable typeof<'T>                -> structuralEqualityComparer fsEqualityComparerUnlimitedHashingPER
        | _ when er                                              -> unknownEqualityComparer true fsEqualityComparerUnlimitedHashingER
        | _                                                      -> unknownEqualityComparer false fsEqualityComparerUnlimitedHashingPER

    [<AbstractClass; Sealed>]
    type FSharpEqualityComparer_ER<'T> private () =
        static let comparer = getGenericEquality<'T> true
        static member EqualityComparer = comparer

    [<AbstractClass; Sealed>]
    type FSharpEqualityComparer_PER<'T> private () =
        static let comparer = getGenericEquality<'T> false
        static member EqualityComparer = comparer

    let inline FSharpEqualityComparer_ER_Equals (x:'T) (y:'T) = 
        FSharpEqualityComparer_ER<'T>.EqualityComparer.Equals (x, y)

    let inline FSharpEqualityComparer_PER_Equals (x:'T) (y:'T) = 
        FSharpEqualityComparer_PER<'T>.EqualityComparer.Equals (x, y)

    let inline FSharpEqualityComparer_GetHashCode (x:'T) = 
        FSharpEqualityComparer_PER<'T>.EqualityComparer.GetHashCode x

    /// Implements generic equality between two values, with PER semantics for NaN (so equality on two NaN values returns false)
    //
    // The compiler optimizer is aware of this function  (see use of generic_equality_per_inner_vref in opt.fs)
    // and devirtualizes calls to it based on "T".
    let GenericEqualityIntrinsic (x : 'T) (y : 'T) : bool = 
        FSharpEqualityComparer_PER<'T>.EqualityComparer.Equals (x, y)


    /// Fill in the implementation of CountLimitedHasherPER
    type CountLimitedHasherPER with
        
        interface System.Collections.IEqualityComparer with
            override iec.Equals(x:obj,y:obj) =
                GenericEqualityObj false iec (x,y)
            override iec.GetHashCode(x:obj) =
                iec.nodeCount <- iec.nodeCount - 1
                if iec.nodeCount > 0 then
                    GenericHashParamObj iec  x
                else
                    -1
       
    /// Fill in the implementation of UnlimitedHasherER
    type UnlimitedHasherER with
        interface System.Collections.IEqualityComparer with
            override iec.Equals(x:obj,y:obj) = GenericEqualityObj true iec (x,y)
            override iec.GetHashCode(x:obj) = GenericHashParamObj iec  x
           
    /// Fill in the implementation of UnlimitedHasherPER
    type UnlimitedHasherPER with
        interface System.Collections.IEqualityComparer with
            override iec.Equals(x:obj,y:obj) = GenericEqualityObj false iec (x,y)
            override iec.GetHashCode(x:obj) = GenericHashParamObj iec x


let FastGenericEqualityComparerFromTable<'T> : IEqualityComparer<'T> =
    HashCompare.FSharpEqualityComparer_PER<'T>.EqualityComparer :> IEqualityComparer<'T>
