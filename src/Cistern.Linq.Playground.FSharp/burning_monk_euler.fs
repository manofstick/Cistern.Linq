module burning_monk_euler

open System.Diagnostics

#if !INCLUDE_PROJECT_INFO
module ProjectInfo =
    let Name = "Unknown"
    let AppendResultTo = None
#endif

[<AutoOpen>]
module Helpers =
    let inline validate expected received =
        if expected <> received then
            failwith <| sprintf "Invalid answer - expected=%A received=%A" expected received

module ``TheBurningMonk - Euler`` =
    // collated from :- https://github.com/theburningmonk/ProjectEuler-FSharp-Solutions/tree/master/ProjectEulerSolutions/ProjectEulerSolutions
    // documentation :- http://theburningmonk.com/project-euler-solutions/
    open System
    open System.Numerics


    let ``1`` () =
        let total = seq { 1..999 } |> Seq.map (fun i -> if i % 5 = 0 || i % 3 = 0 then i else 0) |> Seq.sum

        validate 233168 total

    let ``2`` () =
        let fibonacciSeq = Seq.unfold (fun (current, next) -> Some(current, (next, current + next))) (0, 1)
 
        let fibTotal =
            fibonacciSeq
            |> Seq.takeWhile (fun n -> n < 4000000)
            |> Seq.filter (fun n -> n % 2 = 0)
            |> Seq.sum

        validate 4613732 fibTotal

    let ``3`` () =
        let findFactorsOf(n:int64) =
            let upperBound = int64(Math.Sqrt(double(n)))
            [2L..upperBound] |> Seq.filter (fun x -> n % x = 0L)
 
        let isPrime(n:int64) = findFactorsOf(n) |> Seq.length = 0
 
        let findMaxPrimeFactorOf(n:int64) =
            let upperBound = int64(Math.Sqrt(double(n)))
 
            [2L..upperBound]
            |> Seq.filter (fun x -> n % x = 0L)
            |> Seq.filter isPrime
            |> Seq.max
 
        let maxPrime = findMaxPrimeFactorOf(600851475143L)

        validate 6857L maxPrime

    let ``4`` () =
        let isPalindromic n =
            let charArray = (n.ToString()) // :> seq<char>
            let revCharArray = Seq.rev charArray
            System.Linq.Enumerable.SequenceEqual (charArray, revCharArray)
 
        let numbers = [100..999]
        let products = numbers |> Seq.collect (fun x -> numbers |> Seq.map (fun y -> x * y))
        let maxPalindromic = products |> Seq.filter isPalindromic |> Seq.max

        validate 906609 maxPalindromic

    let ``5`` () =
        let isEvenlyDivided(n, m) = n % m = 0
        let isEvenlyDividedByAll(n, numbers) = numbers |> Seq.forall (fun x -> isEvenlyDivided(n, x))
 
        let findSmallestCommonMultiple(numbers) =
            let max = Array.max(numbers)
            Seq.unfold (fun x -> Some(x, x + 1)) 1
            |> Seq.map (fun x -> x * max)
            |> Seq.filter (fun x -> isEvenlyDividedByAll(x, numbers))
            |> Seq.head
 
        let commonMultiplier = findSmallestCommonMultiple [|1..20|]

        validate 232792560 commonMultiplier

    let ``6`` () =
        let numbers = [|1..100|]
 
        let sumOfSquares = numbers |> Seq.map (fun x -> x * x) |> Seq.sum
 
        let sum = numbers |> Seq.sum
        let squareOfSum = sum * sum
 
        let diff = squareOfSum - sumOfSquares

        validate 25164150 diff

    let ``7`` () =
        let findFactorsOf(n) =
            let upperBound = int32(Math.Sqrt(double(n)))
            [2..upperBound]
            |> Seq.filter (fun x -> n % x = 0)
 
        let isPrime(n) = findFactorsOf(n) |> Seq.length = 0
        let primeNumbers = Seq.unfold (fun x -> Some(x, x + 1)) 2 |> Seq.filter isPrime
        let p = primeNumbers |> Seq.nth(10000)

        validate 104743 p

    let _8_numbers =
        @"73167176531330624919225119674426574742355349194934
    96983520312774506326239578318016984801869478851843
    85861560789112949495459501737958331952853208805511
    12540698747158523863050715693290963295227443043557
    66896648950445244523161731856403098711121722383113
    62229893423380308135336276614282806444486645238749
    30358907296290491560440772390713810515859307960866
    70172427121883998797908792274921901699720888093776
    65727333001053367881220235421809751254540594752243
    52584907711670556013604839586446706324415722155397
    53697817977846174064955149290862569321978468622482
    83972241375657056057490261407972968652414535100474
    82166370484403199890008895243450658541227588666881
    16427171479924442928230863465674813919123162824586
    17866458359124566529476545682848912883142607690042
    24219022671055626321111109370544217506941658960408
    07198403850962455444362981230987879927244284909188
    84580156166097919133875499200524063689912560717606
    05886116467109405077541002256983155200055935729725
    71636269561882670428252483600823257530420752963450"
        |> Seq.filter Char.IsDigit
        |> Seq.map (fun c -> int32(c.ToString()))

    let ``8`` () =
        let numbers = _8_numbers

        let CalcProduct numbers = numbers |> Seq.fold (fun acc n -> acc * n) 1
 
        let maxProduct =
            numbers
            |> Seq.windowed(5)
            |> Seq.map (fun n -> CalcProduct n)
            |> Seq.max

        validate 40824 maxProduct

    let ``9`` () =
        let isPythagoreanTriplet(numbers : int list) =
            match List.sort(numbers) with
            | [a; b; c] -> a*a + b*b = c*c
            | _ -> false
 
        let getTriplets =
            seq {
                for a = 1 to 1000 do
                    for b = 1 to 1000 do
                        for c = 1 to 1000 do
                            if a + b + c = 1000 then yield [a; b; c]
            }
 
        let pythagoreanTriplet = getTriplets |> Seq.filter isPythagoreanTriplet |> Seq.head
        let product = pythagoreanTriplet |> Seq.fold (fun acc x -> acc * x) 1

        validate 31875000 product

    let ``10`` () =
        let findFactorsOf(n:int64) =
            let upperBound = int64(Math.Sqrt(double(n)))
            [2L..upperBound] |> Seq.filter (fun x -> n % x = 0L)
 
        let isPrime(n:int64) = findFactorsOf(n) |> Seq.length = 0
 
        let primeSequence max = seq { for n in 2L..max do if isPrime(n) then yield n }
        let sum = primeSequence 1999999L |> Seq.sum

        validate 142913828922L sum

    let _11_data =
        [   "08 02 22 97 38 15 00 40 00 75 04 05 07 78 52 12 50 77 91 08"
            "49 49 99 40 17 81 18 57 60 87 17 40 98 43 69 48 04 56 62 00"
            "81 49 31 73 55 79 14 29 93 71 40 67 53 88 30 03 49 13 36 65"
            "52 70 95 23 04 60 11 42 69 24 68 56 01 32 56 71 37 02 36 91"
            "22 31 16 71 51 67 63 89 41 92 36 54 22 40 40 28 66 33 13 80"
            "24 47 32 60 99 03 45 02 44 75 33 53 78 36 84 20 35 17 12 50"
            "32 98 81 28 64 23 67 10 26 38 40 67 59 54 70 66 18 38 64 70"
            "67 26 20 68 02 62 12 20 95 63 94 39 63 08 40 91 66 49 94 21"
            "24 55 58 05 66 73 99 26 97 17 78 78 96 83 14 88 34 89 63 72"
            "21 36 23 09 75 00 76 44 20 45 35 14 00 61 33 97 34 31 33 95"
            "78 17 53 28 22 75 31 67 15 94 03 80 04 62 16 14 09 53 56 92"
            "16 39 05 42 96 35 31 47 55 58 88 24 00 17 54 24 36 29 85 57"
            "86 56 00 48 35 71 89 07 05 44 44 37 44 60 21 58 51 54 17 58"
            "19 80 81 68 05 94 47 69 28 73 92 13 86 52 17 77 04 89 55 40"
            "04 52 08 83 97 35 99 16 07 97 57 32 16 26 26 79 33 27 98 66"
            "88 36 68 87 57 62 20 72 03 46 33 67 46 55 12 32 63 93 53 69"
            "04 42 16 73 38 25 39 11 24 94 72 18 08 46 29 32 40 62 76 36"
            "20 69 36 41 72 30 23 88 34 62 99 69 82 67 59 85 74 04 36 16"
            "20 73 35 29 78 31 90 01 74 31 49 71 48 86 81 16 23 57 05 54"
            "01 70 54 71 83 51 54 69 16 92 33 48 61 43 52 01 89 19 67 48" ]
        |> Seq.map (fun l -> l.Split(' ') |> Seq.map int32 |> Seq.toArray)
        |> Seq.toArray

    let ``11`` () =
        let initArray = _11_data
 
        let height, width = initArray.Length, initArray |> Seq.map (fun l -> l.Length) |> Seq.max
        let twoDArray = Array2D.init height width (fun i j -> initArray.[i].[j])
 
        let Up (array2D:int[,]) h w n =
            let lowerBound = h-(n-1)
            let upperBound = h
            if lowerBound < 0 || upperBound > height-1 then []
            else [lowerBound..upperBound] |> List.map (fun y -> array2D.[y, w])
 
        let Left (array2D:int[,]) h w n =
            let lowerBound = w-(n-1)
            let upperBound = w
            if lowerBound < 0 || upperBound > width-1 then []
            else [lowerBound..upperBound] |> List.map (fun x -> array2D.[h, x])
 
        let LeftDiag (array2D:int[,]) h w n =
            let lowerWBound = w-(n-1)
            let upperWBound = w
            let lowerHBound = h-(n-1)
            let upperHBound = h
            if lowerWBound < 0 || upperWBound > width-1 || lowerHBound < 0 || upperHBound > height-1 
            then []
            else
                let wCoordinates = [lowerWBound..upperWBound]
                let hCoordinates = [lowerHBound..upperHBound]
                List.map2 (fun y x -> array2D.[y, x]) hCoordinates wCoordinates
 
        let RightDiag (array2D:int[,]) h w n =
            let lowerWBound = w
            let upperWBound = w+(n-1)
            let lowerHBound = h-(n-1)
            let upperHBound = h
            if lowerWBound < 0 || upperWBound > width-1 || lowerHBound < 0 || upperHBound > height-1 
            then []
            else
                let wCoordinates = [lowerWBound..upperWBound]
                let hCoordinates = [lowerHBound..upperHBound] |> List.rev
                List.map2 (fun y x -> array2D.[y, x]) hCoordinates wCoordinates
 
        let quartets =
            seq { for y in 3 .. width-1 do
                    for x in 3 .. height-1 do
                        yield Up twoDArray x y 4
                        yield Left twoDArray x y 4
                        yield LeftDiag twoDArray x y 4
                        yield RightDiag twoDArray x y 4
            }
 
        let CalcProduct numbers = numbers |> Seq.fold (fun acc n -> acc * n) 1
        let maxProduct = quartets |> Seq.map CalcProduct |> Seq.max

        validate 70600674 maxProduct

    let ``12`` () =
        let triangleNumber(n:int64) = [1L..n] |> Seq.sum
 
        let findFactorsOf(n:int64) =
            let upperBound = int64(Math.Sqrt(double(n)))
            [1L..upperBound] 
            |> Seq.filter (fun x -> n % x = 0L) 
            |> Seq.collect (fun x -> [x; n/x])
 
        let naturalNumbers = Seq.unfold (fun x -> Some(x, x+1L)) 1L
 
        let answer =
            naturalNumbers
            |> Seq.map (fun x -> triangleNumber(x))
            |> Seq.filter (fun x -> Seq.length(findFactorsOf(x)) >= 500)
            |> Seq.head

        validate 76576500L answer

    let _13_data = 
        [   "37107287533902102798797998220837590246510135740250"
            "46376937677490009712648124896970078050417018260538"
            "74324986199524741059474233309513058123726617309629"
            "91942213363574161572522430563301811072406154908250"
            "23067588207539346171171980310421047513778063246676"
            "89261670696623633820136378418383684178734361726757"
            "28112879812849979408065481931592621691275889832738"
            "44274228917432520321923589422876796487670272189318"
            "47451445736001306439091167216856844588711603153276"
            "70386486105843025439939619828917593665686757934951"
            "62176457141856560629502157223196586755079324193331"
            "64906352462741904929101432445813822663347944758178"
            "92575867718337217661963751590579239728245598838407"
            "58203565325359399008402633568948830189458628227828"
            "80181199384826282014278194139940567587151170094390"
            "35398664372827112653829987240784473053190104293586"
            "86515506006295864861532075273371959191420517255829"
            "71693888707715466499115593487603532921714970056938"
            "54370070576826684624621495650076471787294438377604"
            "53282654108756828443191190634694037855217779295145"
            "36123272525000296071075082563815656710885258350721"
            "45876576172410976447339110607218265236877223636045"
            "17423706905851860660448207621209813287860733969412"
            "81142660418086830619328460811191061556940512689692"
            "51934325451728388641918047049293215058642563049483"
            "62467221648435076201727918039944693004732956340691"
            "15732444386908125794514089057706229429197107928209"
            "55037687525678773091862540744969844508330393682126"
            "18336384825330154686196124348767681297534375946515"
            "80386287592878490201521685554828717201219257766954"
            "78182833757993103614740356856449095527097864797581"
            "16726320100436897842553539920931837441497806860984"
            "48403098129077791799088218795327364475675590848030"
            "87086987551392711854517078544161852424320693150332"
            "59959406895756536782107074926966537676326235447210"
            "69793950679652694742597709739166693763042633987085"
            "41052684708299085211399427365734116182760315001271"
            "65378607361501080857009149939512557028198746004375"
            "35829035317434717326932123578154982629742552737307"
            "94953759765105305946966067683156574377167401875275"
            "88902802571733229619176668713819931811048770190271"
            "25267680276078003013678680992525463401061632866526"
            "36270218540497705585629946580636237993140746255962"
            "24074486908231174977792365466257246923322810917141"
            "91430288197103288597806669760892938638285025333403"
            "34413065578016127815921815005561868836468420090470"
            "23053081172816430487623791969842487255036638784583"
            "11487696932154902810424020138335124462181441773470"
            "63783299490636259666498587618221225225512486764533"
            "67720186971698544312419572409913959008952310058822"
            "95548255300263520781532296796249481641953868218774"
            "76085327132285723110424803456124867697064507995236"
            "37774242535411291684276865538926205024910326572967"
            "23701913275725675285653248258265463092207058596522"
            "29798860272258331913126375147341994889534765745501"
            "18495701454879288984856827726077713721403798879715"
            "38298203783031473527721580348144513491373226651381"
            "34829543829199918180278916522431027392251122869539"
            "40957953066405232632538044100059654939159879593635"
            "29746152185502371307642255121183693803580388584903"
            "41698116222072977186158236678424689157993532961922"
            "62467957194401269043877107275048102390895523597457"
            "23189706772547915061505504953922979530901129967519"
            "86188088225875314529584099251203829009407770775672"
            "11306739708304724483816533873502340845647058077308"
            "82959174767140363198008187129011875491310547126581"
            "97623331044818386269515456334926366572897563400500"
            "42846280183517070527831839425882145521227251250327"
            "55121603546981200581762165212827652751691296897789"
            "32238195734329339946437501907836945765883352399886"
            "75506164965184775180738168837861091527357929701337"
            "62177842752192623401942399639168044983993173312731"
            "32924185707147349566916674687634660915035914677504"
            "99518671430235219628894890102423325116913619626622"
            "73267460800591547471830798392868535206946944540724"
            "76841822524674417161514036427982273348055556214818"
            "97142617910342598647204516893989422179826088076852"
            "87783646182799346313767754307809363333018982642090"
            "10848802521674670883215120185883543223812876952786"
            "71329612474782464538636993009049310363619763878039"
            "62184073572399794223406235393808339651327408011116"
            "66627891981488087797941876876144230030984490851411"
            "60661826293682836764744779239180335110989069790714"
            "85786944089552990653640447425576083659976645795096"
            "66024396409905389607120198219976047599490197230297"
            "64913982680032973156037120041377903785566085089252"
            "16730939319872750275468906903707539413042652315011"
            "94809377245048795150954100921645863754710598436791"
            "78639167021187492431995700641917969777599028300699"
            "15368713711936614952811305876380278410754449733078"
            "40789923115535562561142322423255033685442488917353"
            "44889911501440648020369068063960672322193204149535"
            "41503128880339536053299340368006977710650566631954"
            "81234880673210146739058568557934581403627822703280"
            "82616570773948327592232845941706525094512325230608"
            "22918802058777319719839450180888072429661980811197"
            "77158542502016545090413245809786882778948721859617"
            "72107838435069186155435662884062257473692284509516"
            "20849603980134001723930671666823555245252804609722"
            "53503534226472524250874054075591789781264330331690" ]
            |> Seq.map BigInteger.Parse
            |> Seq.sum

    let ``13`` () =
        let sum = _13_data
 
        let firstTenDigits = sum.ToString().ToCharArray() |> Seq.take(10) |> Seq.toList

        validate ['5'; '5'; '3'; '7'; '3'; '7'; '6'; '2'; '3'; '0'] firstTenDigits

    let ``14`` () =
        let nextNumber n = if n%2L = 0L then n/2L else 3L*n+1L
 
        let findSequenceLength n =
            let mutable count = 1L
            let mutable current = n
 
            while current > 1L do
                current <- nextNumber current
                count <- count + 1L
            count
 
        let longestSeq = [1L..999999L] |> Seq.maxBy findSequenceLength

        validate 837799L longestSeq

    let ``16`` () =
        let number = 2I ** 1000
        let answer = number.ToString() |> Seq.map (fun c -> int32(c.ToString())) |> Seq.sum

        validate 1366 answer

    let ``17`` () =
        let onesToWord prefix n postfix =
            match n with
            | 1 -> prefix + "one" + postfix
            | 2 -> prefix + "two" + postfix
            | 3 -> prefix + "three" + postfix
            | 4 -> prefix + "four" + postfix
            | 5 -> prefix + "five" + postfix
            | 6 -> prefix + "six" + postfix
            | 7 -> prefix + "seven" + postfix
            | 8 -> prefix + "eight" + postfix
            | 9 -> prefix + "nine" + postfix
            | _ -> ""
 
        let tensToWord prefix tens ones =
            match tens with
            | 0 ->  onesToWord prefix ones ""
            | 1 -> match ones with
                   | 0 -> prefix + "ten"
                   | 1 -> prefix + "eleven"
                   | 2 -> prefix + "twelve"
                   | 3 -> prefix + "thirteen"
                   | 4 -> prefix + "fourteen"
                   | 5 -> prefix + "fifteen"
                   | 6 -> prefix + "sixteen"
                   | 7 -> prefix + "seventeen"
                   | 8 -> prefix + "eighteen"
                   | 9 -> prefix + "nineteen"
                   | _ -> ""
            | 2 -> prefix + "twenty" + (onesToWord "" ones "")
            | 3 -> prefix + "thirty" + (onesToWord "" ones "")
            | 4 -> prefix + "forty" + (onesToWord "" ones "")
            | 5 -> prefix + "fifty" + (onesToWord "" ones "")
            | 6 -> prefix + "sixty" + (onesToWord "" ones "")
            | 7 -> prefix + "seventy" + (onesToWord "" ones "")
            | 8 -> prefix + "eighty" + (onesToWord "" ones "")
            | 9 -> prefix + "ninety" + (onesToWord "" ones "")
            | _ -> ""
 
        let toWord n =
            let thousands = n / 1000
            let hundreds = (n - 1000 * thousands) / 100
            let tens = (n - 1000 * thousands - 100 * hundreds) / 10
            let ones = n % 10
 
            let thousandsWord = onesToWord "" thousands "thousand"
            let hundredsWord = onesToWord "" hundreds "hundred"
            let tensPrefix = if (thousands > 0 || hundreds > 0) && (tens > 0 || ones > 0)
                             then "and"
                             else ""
            let tensWord = tensToWord tensPrefix tens ones
 
            thousandsWord + hundredsWord + tensWord
        let answer = [1 .. 1000] |> Seq.map toWord |> Seq.sumBy (fun s -> s.Length)

        validate 21124 answer

    let _18_data =
        [|
            "75"
            "95 64"
            "17 47 82"
            "18 35 87 10"
            "20 04 82 47 65"
            "19 01 23 75 03 34"
            "88 02 77 73 07 63 67"
            "99 65 04 28 06 16 70 92"
            "41 41 26 56 83 40 80 70 33"
            "41 48 72 33 47 32 37 16 94 29"
            "53 71 44 65 25 43 91 52 97 51 14"
            "70 11 33 28 77 73 17 78 39 68 17 57"
            "91 71 52 38 17 14 91 43 58 50 27 29 48"
            "63 66 04 68 89 53 67 30 73 16 69 87 40 31"
            "04 62 98 27 23 09 70 98 73 93 38 53 60 04 23"
        |]
        |> Array.map (fun s -> s.Split(' ') |> Array.map int32 |> Array.toList)
        |> Array.toList
 
    let ``18`` () =
        // covers the data in the text to a triangle of ints, i.e. int list list
        let triangle =_18_data
 
        // function to return all the combinations of n elements from the supplied list
        let rec comb n list =
            match n, list with
            | 0, _ -> [[]]
            | _, [] -> []
            | k, (x::xs) -> List.map ((@) [x]) (comb (k-1) xs) @ (comb k xs)
 
        // calculates the next row in the T triangle given the new row in R and the last row in T
        let getNewTotal (row:int list) (total:int list) =
            let head = total.Head
            let tail = List.nth total (total.Length-1)
            let body = total |> Seq.windowed 2 |> Seq.map (fun l -> Seq.max l) |> Seq.toList
            List.map2 (+) row (List.concat [[head]; body; [tail]])
 
        // recursively traverse down the R triangle and return the last row in T
        let rec traverse (raw:int list list) (total:int list) n =
            let row = raw.[n]
            let newTotal = getNewTotal row total
 
            if n < (raw.Length-1) then
                traverse raw newTotal (n+1)
            else
            newTotal
 
        let answer = List.max (traverse triangle [75] 1)

        validate 1074 answer

    let ``19`` () =
        let ans =
            seq {1901..2000}
            |> Seq.collect (fun y -> seq { 1..12 } |> Seq.map (fun m -> new DateTime(y, m, 1)))
            |> Seq.filter (fun d -> d.DayOfWeek = DayOfWeek.Sunday)
            |> Seq.length

        validate 171 ans

    let ``20`` () =
        let rec factorial (n:bigint) = if n = 1I then 1I else n * factorial(n-1I)
 
        let number = factorial 100I
        let digits = number.ToString().ToCharArray() |> Seq.map (fun c -> int32(c.ToString()))
        let sum = digits |> Seq.sum

        validate 648 sum

module ``https%3a%2f%2fgithub%2ecom%2fnessos%2fLinqOptimizer%2fblob%2ff02172b0b87c3688418e3d77d62074d001686baa%2fbenchmarks%2fLinqOptimizer%2eBenchmarks%2eFSharp%2fProgram%2efs%23L44-L49`` =
    // https://github.com/nessos/LinqOptimizer/blob/f02172b0b87c3688418e3d77d62074d001686baa/benchmarks/LinqOptimizer.Benchmarks.FSharp/Program.fs#L44-L49
    let GroupLinq(values : double[]) =
        values
        |> Seq.groupBy(fun x -> int x / 100)
        |> Seq.sortBy (fun (key, vs) -> key)
        |> Seq.map(fun (key, vs) -> Seq.length vs)
        |> Seq.toArray

    let rnd = System.Random 42
    let values = { 1 .. 200000 } |> Seq.map (fun x -> 100000000. * rnd.NextDouble() - 50000000.) |> Seq.toArray

    let test () =
        let mutable total = LanguagePrimitives.GenericZero
        let mutable idx = 1
        for n in GroupLinq values do
             total <- total + n * idx
             idx <- idx + 1
        validate total 943462037

module ``https%3a%2f%2fgithub%2ecom%2fnessos%2fLinqOptimizer%2fblob%2ff02172b0b87c3688418e3d77d62074d001686baa%2fbenchmarks%2fLinqOptimizer%2eBenchmarks%2eFSharp%2fProgram%2efs%23L61-L69`` =
    // https://github.com/nessos/LinqOptimizer/blob/f02172b0b87c3688418e3d77d62074d001686baa/benchmarks/LinqOptimizer.Benchmarks.FSharp/Program.fs#L61-L69

    let PythagoreanTriplesLinq(max) =
        { 1 .. max }
        |> Seq.collect(fun a ->
            { a .. max }
            |> Seq.collect(fun b ->
                { b .. max }
                |> Seq.map (fun c -> a, b, c)))
        |> Seq.filter (fun (a,b,c) -> a * a + b * b = c * c)
        |> Seq.length

    let test () =
        let total = PythagoreanTriplesLinq 200
        validate total 127

module ``https%3a%2f%2fgithub%2ecom%2fnessos%2fLinqOptimizer%2fblob%2ff02172b0b87c3688418e3d77d62074d001686baa%2fbenchmarks%2fLinqOptimizer%2eBenchmarks%2eFSharp%2fProgram%2efs%23L37-L38`` =
    // https://github.com/nessos/LinqOptimizer/blob/f02172b0b87c3688418e3d77d62074d001686baa/benchmarks/LinqOptimizer.Benchmarks.FSharp/Program.fs#L37-L38

    let CartLinq (dim1 : double[], dim2 : double[]) =
        dim1 |> Seq.collect (fun x -> Seq.map (fun y -> x * y) dim2) |> Seq.sum

    let rnd = System.Random 42
    let v = { 1 ..  200000 } |> Seq.map (fun x -> rnd.NextDouble()) |> Seq.toArray
    let v1 = v |> Seq.take(v.Length / 10) |> Seq.toArray
    let v2 = v |> Seq.take 20 |> Seq.toArray

    let test () =
        let total = CartLinq (v1, v2)
        validate (int64 (total*100000.)) 7958952119L

        //"https://github.com/nessos/LinqOptimizer/blob/f02172b0b87c3688418e3d77d62074d001686baa/benchmarks/LinqOptimizer.Benchmarks.FSharp/Program.fs#L37-L38", ``https%3a%2f%2fgithub%2ecom%2fnessos%2fLinqOptimizer%2fblob%2ff02172b0b87c3688418e3d77d62074d001686baa%2fbenchmarks%2fLinqOptimizer%2eBenchmarks%2eFSharp%2fProgram%2efs%23L37-L38``.test, 1

module ``http%3a%2f%2fwww%2efssnip%2enet%2fw%2ftitle%2fUnfolding-Sequences`` =
    // http://www.fssnip.net/w/title/Unfolding-Sequences

    // create an infinite list of fibonacci numbers
    let fibs =
        Seq.unfold
            (fun (n0, n1) ->
                Some(n0, (n1, n0 + n1)))
            (1I,1I)

    // take the first twenty items from the list
    let first20 = Seq.take 20 fibs

    //// print the finite list
    //printfn "%A" first20

    let test () =
        let mutable total = LanguagePrimitives.GenericZero
        for n in first20 do
             total <- total + n
        validate total 17710I

module ``http%3a%2f%2fwww%2efssnip%2enet%2f16%2ftitle%2fSequence-Random-Permutation`` =
    // http://www.fssnip.net/16/title/Sequence-Random-Permutation

    open System

    let scramble (sqn : seq<'T>) = 
        let rnd = Random 42
        let rec scramble2 (sqn : seq<'T>) = 
            /// Removes an element from a sequence.
            let remove n sqn = sqn |> Seq.filter (fun x -> x <> n)
 
            seq {
                let x = sqn |> Seq.nth (rnd.Next(0, sqn |> Seq.length))
                yield x
                let sqn' = remove x sqn
                if not (sqn' |> Seq.isEmpty) then
                    yield! scramble2 sqn'
            }
        scramble2 sqn
 
    // Example:
    let test' () = scramble ['1' .. '9'] |> Seq.toList
    // Output:
    // val test : char list = ['3'; '6'; '7'; '5'; '4'; '8'; '2'; '1'; '9']

    let test () =
        let mutable total = LanguagePrimitives.GenericZero
        let mutable idx = 0
        for n in test' () do
            idx <- idx + 1
            total <- total + (idx * int n)
        validate total 2406

module ``http%3a%2f%2fwww%2efssnip%2enet%2f1f%2ftitle%2fCartesian-Product-of-Sequences`` =
    // http://www.fssnip.net/1f/title/Cartesian-Product-of-Sequences

    // Cartesian product of a sequence of sequences.
    let rec cartSeq (nss:seq<#seq<'a>>) = 
      let f0 (n:'a) (nss:seq<#seq<'a>>) = 
        match Seq.isEmpty nss with
        | true -> Seq.singleton(Seq.singleton n)
        | _ -> Seq.map (fun (nl:#seq<'a>)->seq{yield n; yield! nl}) nss
      match Seq.isEmpty nss with
      | true -> Seq.empty
      | _ -> Seq.collect (fun n->f0 n (cartSeq (Seq.skip 1 nss))) (Seq.head nss)
 
 
    // Test.

    let choices = 
      [
        ["crispy";"thick";"deep-dish";];
        ["pepperoni";"sausage";];
        ["onions";"peppers";];
        ["mozzarella";"provolone";"parmesan"];
      ] 
 
    let pizzas = cartSeq choices

    //pizzas |> Seq.iter (printfn "%A")

    let test () =
        let mutable total = LanguagePrimitives.GenericZero
        let mutable idx = 1
        for toppings in pizzas do
            for topping in toppings do
                total <- total + topping.Length * idx
                idx <- idx + 1
        validate total 80142

module ``http%3a%2f%2fwww%2efssnip%2enet%2f1o%2ftitle%2fBreak-sequence-into-nelement-subsequences`` =
    // http://www.fssnip.net/1o/title/Break-sequence-into-nelement-subsequences

    // Rotaerk's implementation is the fastest and least complicated, the rest are
    // just curiosities unless a bright idea strikes. Compare the performance:
    (* 
    > for x in [1..1000000] |> groupsOfAtMost 500 do printf"";;
    > Real: 00:00:00.423, CPU: 00:00:00.421, GC gen0: 14, gen1: 6, gen2: 0
    val it : unit = ()
    > for x in [1..1000000] |> breakByV3 500 do printf"";;
    > Real: 00:01:04.181, CPU: 00:01:03.991, GC gen0: 15, gen1: 4, gen2: 0
    val it : unit = ()
    > *)
    let groupsOfAtMost (size: int) (s: seq<'v>) : seq<list<'v>> =
        seq {
            let en = s.GetEnumerator ()
            let more = ref true
            while !more do
            let group =
                [
                let i = ref 0
                while !i < size && en.MoveNext () do
                    yield en.Current
                    i := !i + 1
                ]
            if List.isEmpty group then
                more := false
            else
                yield group
        }


    // the original breakBy, made more idiomatic with Rotaerk's help
    let breakByV1 n s = 
        let filter k (i,x) = ((i/n) = k)
        let index = Seq.mapi (fun i x -> (i,x))
        let rec loop s = 
            seq { if not (Seq.isEmpty s) then 
                    let k = (s |> Seq.head |> fst) / n
                    yield (s |> Seq.truncate n
                                |> Seq.map snd)
                    yield! loop (s |> Seq.skipWhile (filter k)) }
        loop (s |> index)

    // with even greater Rotaerk's help, breakBy is now shorter and a couple useful
    // util functions materialize
    let tuple2 x y = x, y
    let trim n = Seq.map snd << Seq.filter (fst >> (<=) n) << Seq.mapi tuple2
    //val it : seq<int> = seq [51; 52; 53; 54; ...]

    let breakByV2 n s = 
        let rec loop s = 
            seq { if not (Seq.isEmpty s) then 
                    yield (s |> Seq.truncate n)
                    yield! loop (s |> trim n) }
        loop s
    
    // in discussions with Rotaerk, it came out that it would be useful to return
    // both first n elements and remaining sequence, in order to iterate seq in one
    // pass. Rotaerk liked the name "trim" for that function, I decided on "spill".
    // dgfitch helped me pinpoint the problem with spill and led me to add |> Seq.cache
    // also this last version returns a sequence of lists, unavoidably I'm afraid.
    // Well, I could wrap lists in seqs but that's just sugar.
    // Changed spill to return option because the way I use Seq.cache eats memory.
    let spill (n:int) (s:seq<'a>)  = 
        let en = s.GetEnumerator()
        let pos = ref 0
        let lst = [ while !pos < n && en.MoveNext() do 
                        pos := !pos+1  
                        yield en.Current]
        if lst |> List.isEmpty then None else
            Some((lst, seq { while en.MoveNext() do yield en.Current}))


    // now breakBy is a true one-liner 
    let breakByV3 n = Seq.unfold (spill n)

    let test1' () = seq {1..2500} |> groupsOfAtMost 50
    let test2' () = seq {1..2500} |> breakByV1 50
    let test3' () = seq {1..2500} |> breakByV2 50
    let test4' () = seq {1..2500} |> breakByV3 50

    let run f validation =
        let mutable total = LanguagePrimitives.GenericZero
        let mutable idx = 0
        for x in f () do
            for y in x do
                total <- total + y * idx
                idx <- idx + 1
        validate total validation

    let test1 () = run test1' 913365204
    let test2 () = run test2' 913365204
    let test3 () = run test3' 913365204
    let test4 () = run test4' 913365204

module ``http%3a%2f%2fwww%2efssnip%2enet%2f1I%2ftitle%2fPartition-a-sequence-until-a-predicate-is-satiated`` =
    type FactAttribute () =
        inherit System.Attribute ()

    type Assert =
        static member Equal (l:list<_>, r:seq<_>) =
            let mutable a = l
            let n = r.GetEnumerator ()
            while n.MoveNext () && not a.IsEmpty do
                if n.Current <> a.Head then failwith "boom"
                a <- a.Tail
            if n.MoveNext () || not a.IsEmpty then
                failwith "boom"

        static member Empty n =
            if not (Seq.isEmpty n) then failwith "boom"

    // http://www.fssnip.net/1I/title/Partition-a-sequence-until-a-predicate-is-satiated
    module Seq =
        /// Takes elements into a sublist until the predicate returns false (exclusive)
        let partitionWhile (func : _ -> bool) (sequence : _ seq) : _ list * _ seq = 
            let en = sequence.GetEnumerator ()
            let wasGood = ref true
            let sublist = 
                [
                    while !wasGood && en.MoveNext() do
                        if func en.Current then yield en.Current
                        else wasGood := false
                ]
            let remainder = 
                seq { 
                        if not !wasGood then yield en.Current
                        while en.MoveNext() do yield en.Current 
                    }
            sublist, remainder

        ///Takes elements into a sublist until the predicate returns true (exclusive)
        let partitionUntil (func : _ -> bool) (sequence : _ seq) : _ list * _ seq = 
            let en = sequence.GetEnumerator ()
            let satisfied = ref false
            let sublist = 
                [
                    while not !satisfied && en.MoveNext() do
                        if not (func en.Current) then yield en.Current
                        else satisfied := true
                ]
            let remainder = 
                seq { 
                        if !satisfied then yield en.Current
                        while en.MoveNext() do yield en.Current 
                    }
            sublist, remainder

        ///Takes elements into a sublist until the predicate returns true (inclusive)
        let partitionUntilAfter (func : _ -> bool) (sequence : _ seq) : _ list * _ seq = 
            let en = sequence.GetEnumerator ()
            let satisfied = ref false
            let sublist = 
                [
                    while not !satisfied && en.MoveNext() do
                        if func en.Current then satisfied := true
                        yield en.Current
                ]
            let remainder = 
                seq { 
                        while en.MoveNext() do yield en.Current 
                    }
            sublist, remainder

    [<Fact>]
    let ``Seq.partitionWhile should return a proper subset and remainder`` () =
        let testSeq = seq { for i in 1 .. 6 do yield i }
        let sub, remainder = testSeq |> Seq.partitionWhile (fun x -> x <= 3)
        Assert.Equal( [1; 2; 3], sub )
        Assert.Equal( [4; 5; 6], remainder |> Seq.toList )

    [<Fact>] 
    let ``Seq.partitionWhile should return an empty list and remainder when give an empty sequence`` () =
        let testSeq = Seq.empty
        let sub, remainder = testSeq |> Seq.partitionWhile (fun x -> x <= 3)
        Assert.Empty sub
        Assert.Empty remainder

    [<Fact>]
    let ``Seq.partitionUntil should return a proper subset and remainder`` =
        let testSeq = seq { for i in 1 .. 6 do yield i }
        let sub, remainder = testSeq |> Seq.partitionUntil (fun x -> x > 3)
        Assert.Equal( [1; 2; 3], sub )
        Assert.Equal( [4; 5; 6], remainder |> Seq.toList )

    [<Fact>] 
    let ``Seq.partitionUntil should return an empty list and remainder when give an empty sequence`` () =
        let testSeq = Seq.empty
        let sub, remainder = testSeq |> Seq.partitionUntil (fun x -> x > 3)
        Assert.Empty sub
        Assert.Empty remainder

    [<Fact>]
    let ``Seq.partitionUntilAfter should return a proper subset and remainder`` () =
        let testSeq = seq { for i in 1 .. 6 do yield i }
        let sub, remainder = testSeq |> Seq.partitionUntilAfter (fun x -> x > 2)
        Assert.Equal( [1; 2; 3], sub )
        Assert.Equal( [4; 5; 6], remainder |> Seq.toList )

    [<Fact>] 
    let ``Seq.partitionUntilAfter should return an empty list and remainder when give an empty sequence`` () =
        let testSeq = Seq.empty
        let sub, remainder = testSeq |> Seq.partitionUntilAfter (fun x -> x > 2)
        Assert.Empty sub
        Assert.Empty remainder

    let test1 () = ``Seq.partitionWhile should return a proper subset and remainder`` ()
    let test2 () = ``Seq.partitionWhile should return an empty list and remainder when give an empty sequence`` ()
    let test3 () = ``Seq.partitionUntil should return a proper subset and remainder``
    let test4 () = ``Seq.partitionUntil should return an empty list and remainder when give an empty sequence`` ()
    let test5 () = ``Seq.partitionUntilAfter should return a proper subset and remainder`` ()
    let test6 () = ``Seq.partitionUntilAfter should return an empty list and remainder when give an empty sequence`` ()

module ``http%3a%2f%2fwww%2efssnip%2enet%2f1N%2ftitle%2fFunction-to-generate-circular-infinite-sequence-from-a-list`` =
    // http://www.fssnip.net/1N/title/Function-to-generate-circular-infinite-sequence-from-a-list
    open System
    open System.IO

    let generateCircularSeq (lst:'a list) = 
        let rec next () = 
            seq {
                for element in lst do
                    yield element
                yield! next()
            }
        next()

    let test () =
        let mutable total = LanguagePrimitives.GenericZero
        let mutable idx = 0
        for i in [1;2;3;4;5;6;7;8;9;10] |> generateCircularSeq |> Seq.take 12 do
            total <- total + i * idx 
            idx <- idx + 1
        validate total 362

module ``http%3a%2f%2fwww%2efssnip%2enet%2f2n%2ftitle%2fSequnsort`` =
    // http://www.fssnip.net/2n/title/Sequnsort
    module Seq =
        let unsort xs =
            let rand = System.Random(Seed=0)
            xs
            |> Seq.map (fun x -> rand.Next(),x)
            |> Seq.cache
            |> Seq.sortBy fst
            |> Seq.map snd

    let values = [| 1..100 |]

    let test () =
        let mutable total = LanguagePrimitives.GenericZero
        let mutable idx = 1
        for n in Seq.unsort values do
             total <- total + n * idx
             idx <- idx + 1
        validate total 248174

module ``http%3a%2f%2fwww%2efssnip%2enet%2f4u%2ftitle%2fVery-Fast-Permutations`` =
    // http://www.fssnip.net/4u/title/Very-Fast-Permutations
    module List = 

        // From: http://stackoverflow.com/questions/286427/calculating-permutations-in-f
        // Much faster than anything else I've tested

        let rec insertions x = function
            | []             -> [[x]]
            | (y :: ys) as l -> (x::l)::(List.map (fun x -> y::x) (insertions x ys))

        let rec permutations = function
            | []      -> seq [ [] ]
            | x :: xs -> Seq.concat (Seq.map (insertions x) (permutations xs))

    let values = [1;2;3;4;5;6;7;8;9]

    let test () =
        let mutable total = LanguagePrimitives.GenericZero
        let mutable idx = 1
        for n in List.permutations values do
            for m in n do
                total <- total + m * idx
                idx <- idx + 1
        validate total -1860160064

module ``http%3a%2f%2fwww%2efssnip%2enet%2ftc%2ftitle%2fFibonacci-sequence-with-scan`` =
    // http://www.fssnip.net/tc/title/Fibonacci-sequence-with-scan
    let rec fibonacci = seq { yield 1; yield! Seq.scan (+) 2 fibonacci }

    let test () =
        let mutable total = LanguagePrimitives.GenericZero
        let mutable idx = 1
        for n in fibonacci |> Seq.take 50 do
             total <- total + n * idx
             idx <- idx + 1
        validate total -1728357515

module ``http%3a%2f%2fwww%2efssnip%2enet%2f6H%2ftitle%2fTake-value-from-a-sequence-only-when-it-changes`` =
    // http://www.fssnip.net/6H/title/Take-value-from-a-sequence-only-when-it-changes

    open System.Collections.Generic

    //Filter function with accumulator
    let filter (acc:'a) (f:('a -> 'b -> bool * 'a)) (s:'b seq) = 
        let rec iter (acc:'a) (e:IEnumerator<'b>) = 
            match e.MoveNext() with
            | false -> Seq.empty 
            | true -> match f acc e.Current with
                      | (true,newAcc) -> seq { yield e.Current; yield! iter newAcc e}
                      | (false,newAcc) -> seq { yield! iter newAcc e}
        iter acc (s.GetEnumerator())

    //main function
    let skipUntilChange (f : 'a -> 'b) (s : 'a seq) = 
        s |> Seq.skip 1
        |> filter (s |> Seq.head |> f)
            (fun a b -> if a = f b then false,f b else true,f b)

    let example () =
        //Example:
        [1;1;1;3;3;3;5;5;5] |> skipUntilChange id //|> printfn "%A"

    let test () =
        let mutable total = LanguagePrimitives.GenericZero
        let mutable idx = 1
        for n in example () do
             total <- total + n * idx
             idx <- idx + 1
        validate total 13

module ``http%3a%2f%2fwww%2efssnip%2enet%2f9X%2ftitle%2fnary-Seqmap-Numerals`` =
    // http://www.fssnip.net/9X/title/nary-Seqmap-Numerals

    // For more info: ftp://ftp.cs.au.dk/pub/BRICS/RS/01/10/BRICS-RS-01-10.pdf

    let (<*>) fs xs = Seq.map2 (fun f x -> f x) fs xs
    let succ n fs xs = n (fs <*> xs)
    let map n f = n (Seq.initInfinite (fun _ -> f))

    // Numerals
    let ``1``<'a1, 'r> : seq<('a1 -> 'r)> -> seq<'a1> -> seq<'r> = 
        succ id
    let ``2``<'a1, 'a2, 'r> : seq<('a1 -> 'a2 -> 'r)> -> seq<'a1> -> seq<'a2> -> seq<'r> = 
        succ ``1``
    let ``3``<'a1, 'a2, 'a3, 'r> : seq<('a1 -> 'a2 -> 'a3 -> 'r)> -> seq<'a1> -> seq<'a2> -> seq<'a3> -> seq<'r> = 
        succ ``2``

    // Examples
    let example1 () = map ``1`` (fun x -> x + 1) [1; 2] // [2; 3]
    let example2 () = map ``2`` (fun x y -> x + y) [1; 2] [1; 2] // [2; 4]
    let example3 () = map ``3`` (fun x y z -> x + y + z) [1; 2] [1; 2] [1; 2] // [3; 6]

    let test example checksum =
        let mutable total = LanguagePrimitives.GenericZero
        let mutable idx = 1
        for n in example () do
             total <- total + n * idx
             idx <- idx + 1
        validate total checksum

    let test1 () = test example1 8
    let test2 () = test example2 10
    let test3 () = test example3 15

module ``http%3a%2f%2fwww%2efssnip%2enet%2feq%2ftitle%2fSum-of-Squares-Monoid`` =
    // http://www.fssnip.net/eq/title/Sum-of-Squares-Monoid

    /// Define SumOfSquares computation builder
    type SumOfSquaresMonoid() =
      /// Combine two values
      /// sm.Combine («cexpr1», b.Delay(fun () -> «cexpr2»))
      member sm.Combine(a,b) = a + b
      /// Zero value
      /// sm.Zero()
      member sm.Zero() = 0.0
      /// Return a value 
      /// sm.Yield expr
      member sm.Yield(a) = a
      /// Delay a computation
      /// sm.Delay (fun () -> «cexpr»))
      member sm.Delay f = f()
      /// For loop
      /// sm.For (expr, (fun pat -> «cexpr»))
      member sm.For(e, f) =
        Seq.fold(fun s x -> sm.Combine(s, f x)) (sm.Zero()) e

    // Create an instance of each such monoid object
    let sosm = new SumOfSquaresMonoid()

    // Build a SumOfSquaresMonoid value(function)
    let sumOfSquares x = sosm {for x in [1.0 .. 0.2 .. x] do yield x * x}

    // Result
    // val it : float = 1819.84
    let test () =
        let total = sumOfSquares 10.2
        validate (int (total*100.)) 181984

module ``http%3a%2f%2fwww%2efssnip%2enet%2fhE%2ftitle%2fLargest-palindrome-made-from-the-product-of-two-ndigit-numbers`` =
    // http://www.fssnip.net/hE/title/Largest-palindrome-made-from-the-product-of-two-ndigit-numbers

    let numDigits = 3
    if (numDigits < 1) then failwith "Number of digits must be at least 1" 

    let lowestNumDigitNumber = pown 10 (numDigits - 1)  
    let highestNumDigitNumber = (pown 10 numDigits) - 1
    let baseSeq = {lowestNumDigitNumber..highestNumDigitNumber}  

    let reverse (t : string) =
        new string(t.ToCharArray() |> Array.rev)
    
    let isPalindrome t = reverse (string t) = string t

    let test () =
        let n = 
            Seq.map (fun x -> (Seq.map (fun y -> x * y) baseSeq)) baseSeq
            |> Seq.concat
            |> Seq.filter isPalindrome
            |> Seq.max
        validate n 906609

module ``http%3a%2f%2fwww%2efssnip%2enet%2f7OI%2ftitle%2fFunctional-and-simple-version-for-Collatz-Conjecture-or-3n-1-Problem`` =
    // http://www.fssnip.net/7OI/title/Functional-and-simple-version-for-Collatz-Conjecture-or-3n-1-Problem

    //https://en.wikipedia.org/wiki/Collatz_conjecture
    //https://uva.onlinejudge.org/index.php?option=com_onlinejudge&Itemid=8&page=show_problem&problem=36

    let min a b = if (a > b) then b else a
    let max a b = if (b > a) then b else a
    let f a b =
      let g j =
        seq { 
          let mutable i = j
          while (i > 1) do
            yield i
            i <- if ((i % 2) = 0) then i / 2 else 3 * i + 1
          yield 1
        }
      seq{(min a b)..(max a b)} |> Seq.map g
    let max_length a b = f a b |> Seq.map  (fun x -> x |> Seq.length) |> Seq.max
    let print_all a b  = f a b |> Seq.iter (fun x -> printfn "%A" (x |> Seq.toList))

    (*

    > print_all 1 10;;
    [1]
    [2; 1]
    [3; 10; 5; 16; 8; 4; 2; 1]
    [4; 2; 1]
    [5; 16; 8; 4; 2; 1]
    [6; 3; 10; 5; 16; 8; 4; 2; 1]
    [7; 22; 11; 34; 17; 52; 26; 13; 40; 20; 10; 5; 16; 8; 4; 2; 1]
    [8; 4; 2; 1]
    [9; 28; 14; 7; 22; 11; 34; 17; 52; 26; 13; 40; 20; 10; 5; 16; 8; 4; 2; 1]

    > max_length 1 10;;
    val it : int = 20
    > max_length 100 200;;
    val it : int = 125
    > max_length 201 210;;
    val it : int = 89
    > max_length 900 1000;;
    val it : int = 174

    *)

    let test () =
        let mutable n = max_length 900 1000
        validate n 174

module ``http%3a%2f%2fwww%2efssnip%2enet%2f8a%2ftitle%2fGluingup-sequence-members`` =
    open System
    open System.Text

    // Glue up using Seq.collect (worst peformer)
    let glueUp1 len ss =
        new string (
            ss
            |> Seq.collect (fun s -> Seq.ofArray <| s.ToString().ToCharArray())
            |> Seq.take len
            |> Seq.toArray)

    // Glue up using Seq.scan + Seq.skipWhile
    let glueUp2 len ss =
        ss
        |> Seq.scan (fun (accum: StringBuilder) s -> accum.Append(s.ToString())) (StringBuilder())
        |> Seq.skipWhile (fun x -> x.Length < len)
        |> Seq.head |> string |> fun x -> x.Substring(0, len)

    // Glue up using Seq.pick and a closure over StringBuilder (best performer)
    let glueUp3 len ss =
        let glue len =
            let accum = StringBuilder()
            fun item ->
                if accum.Length >= len then
                    Some(accum.ToString())
                else
                    accum.Append(item.ToString()) |> ignore
                    None
    
        ss
        |> Seq.pick(glue len) |> fun x -> x.Substring(0, len)

    (*[omit:(Performance measurement)]*)
    let test1' () = (glueUp1 1000000) (Seq.initInfinite(fun x -> x))
    let test2' () = (glueUp2 1000000) (Seq.initInfinite(fun x -> x))
    let test3' () = (glueUp3 1000000) (Seq.initInfinite(fun x -> x))

    let runtest f =
        let mutable total = LanguagePrimitives.GenericZero
        let mutable idx = 1
        for n in f () do
             total <- total + int n * idx
             idx <- idx + 1
        validate total 1678052085

    let test1 () = runtest test1'
    let test2 () = runtest test2'
    let test3 () = runtest test3'

        //"http://www.fssnip.net/8a/title/Gluingup-sequence-members", ``http%3a%2f%2fwww%2efssnip%2enet%2f8a%2ftitle%2fGluingup-sequence-members``.test, 1

module ``http%3a%2f%2ffable%2eio%2frepl%2fsoduku`` =
    open System
    open System.Collections.Generic

    type Box = int
    type Sudoku = Box array array

    let rows = id
    let cols (sudoku:Sudoku) =
        sudoku
        |> Array.mapi (fun a row -> row |> Array.mapi (fun b cell -> sudoku.[b].[a]))

    let getBoxIndex count row col =
       let n = row/count
       let m = col/count
       n * count + m

    let boxes (sudoku:Sudoku) =
        let d = sudoku |> Array.length |> float |> System.Math.Sqrt |> int
        let list = new List<_>()
        for a in 0..(d*d) - 1 do list.Add(new List<_>())

        for a in 0..(Array.length sudoku - 1) do
            for b in 0..(Array.length sudoku - 1) do
                list.[getBoxIndex d a b].Add(sudoku.[a].[b])

        list
          |> Seq.map Seq.toArray

    let toSudoku x : Sudoku =
        x
        |> Seq.map Seq.toArray
        |> Seq.toArray

    let allUnique numbers =
        let set = new HashSet<_>()
        numbers
        |> Seq.filter ((<>) 0)
        |> Seq.forall set.Add

    let solvable sudoku =
        rows sudoku
        |> Seq.append (cols sudoku)
        |> Seq.append (boxes sudoku)
        |> Seq.forall allUnique

    let replaceAtPos (x:Sudoku) row col newValue :Sudoku =
        [| for a in 0..(Array.length x - 1) ->
            [| for b in 0..(Array.length x - 1) ->
                if a = row && b = col then newValue else x.[a].[b] |] |]

    let rec substitute row col (x:Sudoku) =
        let a,b = if col >= Array.length x then row+1,0 else row,col
        if a >= Array.length x then seq { yield x } else
        if x.[a].[b] = 0 then
            [1..Array.length x]
                |> Seq.map (replaceAtPos x a b)
                |> Seq.filter solvable
                |> Seq.map (substitute a (b+1))
                |> Seq.concat
         else substitute a (b+1) x

    let getFirstSolution = substitute 0 0 >> Seq.head

    let test1 () =
        let problem = 
            [[0; 0; 8;  3; 0; 0;  6; 0; 0]
             [0; 0; 4;  0; 0; 0;  0; 1; 0]
             [6; 7; 0;  0; 8; 0;  0; 0; 0]

             [0; 1; 6;  4; 3; 0;  0; 0; 0]
             [0; 0; 0;  7; 9; 0;  0; 2; 0]
             [0; 9; 0;  0; 0; 0;  4; 0; 1]

             [0; 0; 0;  9; 1; 0;  0; 0; 5]
             [0; 0; 3;  0; 5; 0;  0; 0; 2]
             [0; 5; 0;  0; 0; 0;  0; 7; 4]]

        let expected =
            [| [|1; 2; 8;  3; 4; 5;  6; 9; 7|] 
               [|5; 3; 4;  6; 7; 9;  2; 1; 8|] 
               [|6; 7; 9;  1; 8; 2;  5; 4; 3|] 

               [|2; 1; 6;  4; 3; 8;  7; 5; 9|] 
               [|4; 8; 5;  7; 9; 1;  3; 2; 6|] 
               [|3; 9; 7;  5; 2; 6;  4; 8; 1|] 

               [|7; 6; 2;  9; 1; 4;  8; 3; 5|] 
               [|9; 4; 3;  8; 5; 7;  1; 6; 2|] 
               [|8; 5; 1;  2; 6; 3;  9; 7; 4|] |]


        let received = 
            problem
            |> toSudoku
            |> getFirstSolution

        validate expected received

    let test2 () =
        let problem = 
            [[0; 0; 8;  3; 0; 0;  6; 0; 0]
             [0; 0; 4;  0; 0; 0;  0; 1; 0]
             [6; 7; 0;  0; 8; 0;  0; 0; 0]

             [0; 1; 6;  4; 3; 0;  0; 0; 0]
             [0; 0; 0;  7; 9; 0;  0; 2; 0]
             [0; 9; 0;  0; 0; 0;  4; 0; 1]

             [0; 0; 0;  9; 1; 0;  0; 0; 5]
             [0; 0; 3;  0; 5; 0;  0; 0; 2]
             [0; 5; 0;  0; 0; 0;  0; 7; 4]]

        let received = 
            problem
            |> toSudoku
            |> substitute 0 0
            |> Seq.length

        validate 1 received

let time what f =
    let sw = Stopwatch.StartNew ()
    let count = f 500000 
    printfn "%s: %d (%d)" what sw.ElapsedMilliseconds count

let tests = [|
    "TheBurningMonk - Euler - 1", ``TheBurningMonk - Euler``.``1``, 40000
    "TheBurningMonk - Euler - 2", ``TheBurningMonk - Euler``.``2``, 400000
    "TheBurningMonk - Euler - 3", ``TheBurningMonk - Euler``.``3``, 12
    "TheBurningMonk - Euler - 4", ``TheBurningMonk - Euler``.``4``, 5
    "TheBurningMonk - Euler - 5", ``TheBurningMonk - Euler``.``5``, 2
    "TheBurningMonk - Euler - 6", ``TheBurningMonk - Euler``.``6``, 300000
    "TheBurningMonk - Euler - 7", ``TheBurningMonk - Euler``.``7``, 2
    "TheBurningMonk - Euler - 8", ``TheBurningMonk - Euler``.``8``, 3000
    "TheBurningMonk - Euler - 9", ``TheBurningMonk - Euler``.``9``, 1
    "TheBurningMonk - Euler - 10", ``TheBurningMonk - Euler``.``10``, 1
    "TheBurningMonk - Euler - 11", ``TheBurningMonk - Euler``.``11``, 1500
    "TheBurningMonk - Euler - 12", ``TheBurningMonk - Euler``.``12``, 1
    "TheBurningMonk - Euler - 13", ``TheBurningMonk - Euler``.``13``, 400000
    "TheBurningMonk - Euler - 14", ``TheBurningMonk - Euler``.``14``, 4
    "TheBurningMonk - Euler - 16", ``TheBurningMonk - Euler``.``16``, 10000
    "TheBurningMonk - Euler - 17", ``TheBurningMonk - Euler``.``17``, 10000
    "TheBurningMonk - Euler - 18", ``TheBurningMonk - Euler``.``18``, 40000
    "TheBurningMonk - Euler - 19", ``TheBurningMonk - Euler``.``19``, 10000
    "TheBurningMonk - Euler - 20", ``TheBurningMonk - Euler``.``20``, 20000
    "[Nessos LinqOptimizer 1](https://github.com/nessos/LinqOptimizer/blob/f02172b0b87c3688418e3d77d62074d001686baa/benchmarks/LinqOptimizer.Benchmarks.FSharp/Program.fs#L44-L49)", ``https%3a%2f%2fgithub%2ecom%2fnessos%2fLinqOptimizer%2fblob%2ff02172b0b87c3688418e3d77d62074d001686baa%2fbenchmarks%2fLinqOptimizer%2eBenchmarks%2eFSharp%2fProgram%2efs%23L44-L49``.test, 5
    "[Nessos LinqOptimizer 2](https://github.com/nessos/LinqOptimizer/blob/f02172b0b87c3688418e3d77d62074d001686baa/benchmarks/LinqOptimizer.Benchmarks.FSharp/Program.fs#L61-L69)", ``https%3a%2f%2fgithub%2ecom%2fnessos%2fLinqOptimizer%2fblob%2ff02172b0b87c3688418e3d77d62074d001686baa%2fbenchmarks%2fLinqOptimizer%2eBenchmarks%2eFSharp%2fProgram%2efs%23L61-L69``.test, 10
    "[Nessos LinqOptimizer 3](https://github.com/nessos/LinqOptimizer/blob/f02172b0b87c3688418e3d77d62074d001686baa/benchmarks/LinqOptimizer.Benchmarks.FSharp/Program.fs#L37-L38)", ``https%3a%2f%2fgithub%2ecom%2fnessos%2fLinqOptimizer%2fblob%2ff02172b0b87c3688418e3d77d62074d001686baa%2fbenchmarks%2fLinqOptimizer%2eBenchmarks%2eFSharp%2fProgram%2efs%23L37-L38``.test, 50
    "[Unfolding-Sequences](http://www.fssnip.net/w/title/Unfolding-Sequences)", ``http%3a%2f%2fwww%2efssnip%2enet%2fw%2ftitle%2fUnfolding-Sequences``.test, 400000
    "[Sequence-Random-Permutation](http://www.fssnip.net/16/title/Sequence-Random-Permutation)", ``http%3a%2f%2fwww%2efssnip%2enet%2f16%2ftitle%2fSequence-Random-Permutation``.test, 20000
    "[Cartesian-Product-of-Sequences](http://www.fssnip.net/1f/title/Cartesian-Product-of-Sequences)", ``http%3a%2f%2fwww%2efssnip%2enet%2f1f%2ftitle%2fCartesian-Product-of-Sequences``.test, 6000
    "[Break-sequence-into-nelement-subsequences 1](http://www.fssnip.net/1o/title/Break-sequence-into-nelement-subsequences)", ``http%3a%2f%2fwww%2efssnip%2enet%2f1o%2ftitle%2fBreak-sequence-into-nelement-subsequences``.test1, 10000
    "[Break-sequence-into-nelement-subsequences 2](http://www.fssnip.net/1o/title/Break-sequence-into-nelement-subsequences)", ``http%3a%2f%2fwww%2efssnip%2enet%2f1o%2ftitle%2fBreak-sequence-into-nelement-subsequences``.test2, 10
    "[Break-sequence-into-nelement-subsequences 3](http://www.fssnip.net/1o/title/Break-sequence-into-nelement-subsequences)", ``http%3a%2f%2fwww%2efssnip%2enet%2f1o%2ftitle%2fBreak-sequence-into-nelement-subsequences``.test3, 10
    "[Break-sequence-into-nelement-subsequences 4](http://www.fssnip.net/1o/title/Break-sequence-into-nelement-subsequences)", ``http%3a%2f%2fwww%2efssnip%2enet%2f1o%2ftitle%2fBreak-sequence-into-nelement-subsequences``.test4, 1000
    "[Partition-a-sequence-until-a-predicate-is-satiated 1](http://www.fssnip.net/1I/title/Partition-a-sequence-until-a-predicate-is-satiated)", ``http%3a%2f%2fwww%2efssnip%2enet%2f1I%2ftitle%2fPartition-a-sequence-until-a-predicate-is-satiated``.test1, 800000
    "[Partition-a-sequence-until-a-predicate-is-satiated 2](http://www.fssnip.net/1I/title/Partition-a-sequence-until-a-predicate-is-satiated)", ``http%3a%2f%2fwww%2efssnip%2enet%2f1I%2ftitle%2fPartition-a-sequence-until-a-predicate-is-satiated``.test2, 2500000
    "[Partition-a-sequence-until-a-predicate-is-satiated 3](http://www.fssnip.net/1I/title/Partition-a-sequence-until-a-predicate-is-satiated)", ``http%3a%2f%2fwww%2efssnip%2enet%2f1I%2ftitle%2fPartition-a-sequence-until-a-predicate-is-satiated``.test3, 500000000
    "[Partition-a-sequence-until-a-predicate-is-satiated 4](http://www.fssnip.net/1I/title/Partition-a-sequence-until-a-predicate-is-satiated)", ``http%3a%2f%2fwww%2efssnip%2enet%2f1I%2ftitle%2fPartition-a-sequence-until-a-predicate-is-satiated``.test4, 2500000
    "[Partition-a-sequence-until-a-predicate-is-satiated 5](http://www.fssnip.net/1I/title/Partition-a-sequence-until-a-predicate-is-satiated)", ``http%3a%2f%2fwww%2efssnip%2enet%2f1I%2ftitle%2fPartition-a-sequence-until-a-predicate-is-satiated``.test5, 800000
    "[Partition-a-sequence-until-a-predicate-is-satiated 6](http://www.fssnip.net/1I/title/Partition-a-sequence-until-a-predicate-is-satiated)", ``http%3a%2f%2fwww%2efssnip%2enet%2f1I%2ftitle%2fPartition-a-sequence-until-a-predicate-is-satiated``.test6, 2500000
    "[Function-to-generate-circular-infinite-sequence-from-a-list](http://www.fssnip.net/1N/title/Function-to-generate-circular-infinite-sequence-from-a-list)", ``http%3a%2f%2fwww%2efssnip%2enet%2f1N%2ftitle%2fFunction-to-generate-circular-infinite-sequence-from-a-list``.test, 500000
    "[Sequnsort](http://www.fssnip.net/2n/title/Sequnsort)", ``http%3a%2f%2fwww%2efssnip%2enet%2f2n%2ftitle%2fSequnsort``.test, 50000
    "[Very-Fast-Permutations](http://www.fssnip.net/4u/title/Very-Fast-Permutations)", ``http%3a%2f%2fwww%2efssnip%2enet%2f4u%2ftitle%2fVery-Fast-Permutations``.test, 15
    "[Fibonacci-sequence-with-scan](http://www.fssnip.net/tc/title/Fibonacci-sequence-with-scan)", ``http%3a%2f%2fwww%2efssnip%2enet%2ftc%2ftitle%2fFibonacci-sequence-with-scan``.test, 15000
    "[Take-value-from-a-sequence-only-when-it-changes](http://www.fssnip.net/6H/title/Take-value-from-a-sequence-only-when-it-changes)", ``http%3a%2f%2fwww%2efssnip%2enet%2f6H%2ftitle%2fTake-value-from-a-sequence-only-when-it-changes``.test, 400000
    "[nary-Seqmap-Numerals 1](http://www.fssnip.net/9X/title/nary-Seqmap-Numerals)", ``http%3a%2f%2fwww%2efssnip%2enet%2f9X%2ftitle%2fnary-Seqmap-Numerals``.test1, 1000000
    "[nary-Seqmap-Numerals 2](http://www.fssnip.net/9X/title/nary-Seqmap-Numerals)", ``http%3a%2f%2fwww%2efssnip%2enet%2f9X%2ftitle%2fnary-Seqmap-Numerals``.test2, 750000
    "[nary-Seqmap-Numerals 3](http://www.fssnip.net/9X/title/nary-Seqmap-Numerals)", ``http%3a%2f%2fwww%2efssnip%2enet%2f9X%2ftitle%2fnary-Seqmap-Numerals``.test3, 500000
    "[Sum-of-Squares-Monoid](http://www.fssnip.net/eq/title/", ``http%3a%2f%2fwww%2efssnip%2enet%2feq%2ftitle%2fSum-of-Squares-Monoid``.test, 100000
    "[Largest-palindrome-made-from-the-product-of-two-ndigit-numbers](http://www.fssnip.net/hE/title/Largest-palindrome-made-from-the-product-of-two-ndigit-numbers)", ``http%3a%2f%2fwww%2efssnip%2enet%2fhE%2ftitle%2fLargest-palindrome-made-from-the-product-of-two-ndigit-numbers``.test, 4
    "[Functional-and-simple-version-for-Collatz-Conjecture-or-3n-1-Problem](http://www.fssnip.net/7OI/title/Functional-and-simple-version-for-Collatz-Conjecture-or-3n-1-Problem)", ``http%3a%2f%2fwww%2efssnip%2enet%2f7OI%2ftitle%2fFunctional-and-simple-version-for-Collatz-Conjecture-or-3n-1-Problem``.test, 4000
    "[Gluingup-sequence-members 1](http://www.fssnip.net/8a/title/Gluingup-sequence-members)", ``http%3a%2f%2fwww%2efssnip%2enet%2f8a%2ftitle%2fGluingup-sequence-members``.test1, 6
    "[Gluingup-sequence-members 2](http://www.fssnip.net/8a/title/Gluingup-sequence-members)", ``http%3a%2f%2fwww%2efssnip%2enet%2f8a%2ftitle%2fGluingup-sequence-members``.test2, 14
    "[Gluingup-sequence-members 3](http://www.fssnip.net/8a/title/Gluingup-sequence-members)", ``http%3a%2f%2fwww%2efssnip%2enet%2f8a%2ftitle%2fGluingup-sequence-members``.test3, 15
    "[Soduku - first](http://fable.io/repl)", ``http%3a%2f%2ffable%2eio%2frepl%2fsoduku``.test1, 30
    "[Soduku - count](http://fable.io/repl)", ``http%3a%2f%2ffable%2eio%2frepl%2fsoduku``.test2, 1
|]

let main (argv:array<string>) =
    let appendResultTo, name' = 
        if argv.Length > 0 then
            (Some argv.[0]), argv.[1]
        else
            ProjectInfo.AppendResultTo, ProjectInfo.Name

    let bittage = 
        if sizeof<nativeint> = 4 then "32-bit"
        elif sizeof<nativeint> = 8 then "64-bit"
        else "unknown"

    printf "warming up"

    for _, f, iterations in tests do
        if iterations > 1 then
            f ()
        printf "."
    printfn "\n"

    for i = 1 to 5 do
        for name, f, iterations in tests do
            let sw = Stopwatch.StartNew ()

            for i = 1 to iterations do
                f ()

            System.GC.Collect ()
            System.GC.WaitForPendingFinalizers ()

            let text = sprintf "%s,%s,%s,%d\n" name' bittage name sw.ElapsedMilliseconds

            System.Console.Write text

            appendResultTo
            |> Option.iter (fun filename -> System.IO.File.AppendAllText (filename, text))

    0