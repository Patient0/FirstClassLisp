; This file contains test cases used in EvaluatorTest. It is expected to
; contain a list of lists.
; Each list is a test case.
; The first element is the name
; The second is expected result.
; The third is the expression to evaluate.
(atom 5 5)
(boolAtomTrue #t #t)
(boolAtomFalse #f #f)
; Many of these tests require an arbitrary symbol
; to be defined. So we'll use 'life' as our symbol,
; which is mapped to the integer '42' in the environment.
(symbol 42 life)
(identityFunction 5 ((lambda (x) x) 5))
(constantFunction 6 ((lambda () 6)))
(lambdaList (1 2 3) ((lambda x x) 1 2 3))
(list (1 2 3) (list 1 2 3))
(cons (3 . 4) (cons 3 4))
(recursiveFunctions (3 4 . 5) (cons 3 (cons 4 5)))
(apply (3 . 4) (apply cons (list 3 4)))
(dotArgList 4 ((lambda (x . y) x) 4))
(carList 4 (car (list 4 5)))
(cdrList (5) (cdr (list 4 5)))
(eqTrue? #t
		(eq? 4 4))
(eqFalse? #f
		(eq? 4 3))
(eqSymbolTest #t
    (eq? 'somesymbol 'somesymbol))
(eqSymbolTestFalse #f
    (eq? 'someSymbol 'someOtherSymbol))
(eqQuotedSymbolWithUnquoted #t
    (let x 'unquote
        (eq? x 'unquote)))
; But we don't want eq? to be traversing
; arbitrarily large structures for us.
(eqIsFlat #f
		(eq? (list 1 2) (list 1 2)))
(ifTrue? 5
		(if #t 5 undefined))
(ifFalse? 5
		(if #f undefined 5))
; Discovered quite late on that the if
; F-expression wasn't actually evaluating either the
; true or the false cases!
(ifExpressionTrue 42 
        (if (eq? life 42) life 23))
(ifExpressionFalse 42 
        (if (eq? life 98) 5 life))
; We'll make our 'if' like a 'cond' - allow multiple
; clauses until one matches. Should always be an
; odd number of clauses.
(ifIsLikeCond 42
        (if (eq? life 23) not-this-one
            (eq? life 44) not-this-one
            (eq? life 48) no-not-this-either
            42))
(quotedList (3 4) '(3 4))
(append (1 2 3 4)
    (append '(1 2) '(3 4)))
(appendMultiple (1 2 3 4 5 6)
    (append '(1 2) '(3 4) '(5 6)))
; It's very convenient if append works even for a single list
(appendSingle (1 2)
    (append '(1 2)))
(appendNone ()
    (append))
(quotedAtom 3 '3)
(quotedQuote '3 ''3)
(quotedSymbol x 'x)
(let 3 (let x 3 x))
(letEvaluatesBody #t
		(let x 3 (eq? x 3)))
; Our nil *only* matches nil. Anything else is "not nil")
(nil1 #t (nil? '()))
(nil2 #f (nil? 5))
(nil3 #f (nil? #f))
(pair1 #t (pair? '(1 . 2)))
(pair2 #f (pair? 5))
(foldrSimplest 10
    (fold-right + 10 '()))
(foldrTest 25
    (fold-right + 10 '(1 2 3 4 5)))

(mapCarSimplest ()
    (mapcar (lambda (x) (* x x)) '()))
(mapCarTest (1 4 9)
    (mapcar (lambda (x) (* x x)) '(1 2 3)))

(mapSquare (1 4 9)
    (map (lambda (x) (* x x)) '(1 2 3)))
(mapTest (4 10 18)
    (map * '(1 2 3) '(4 5 6)))
(mapThreeTest (28 80 162)
 (map * '(1 2 3) '(4 5 6) '(7 8 9)))

; Using the implicit stack for evaluation here would
; lead to stack overflow. But because this code is
; properly tail recursive and we are using an explicit
; stack, this test runs in constant memory.
(properTailRecursion
    500500
    (define sum-to-1000
        (define total
            (lambda (so-far 0) so-far
                    (so-far x) (total (+ so-far x) (- x 1))))
        (total 0 1000)))
; Now that we've allowed mutable state, we need a way to execute multiple
; statements in sequence.
(begin
    23
    (begin
        (define x 23)
        x))

; The forms in a begin statement execute
; in their own scoped environment - changes
; to that environment are not seen outside
; the environment.
(beginScoping
    24
    (begin
        (define x 24)
        (begin
            (define x 48))
        x))

(lengthTest 3
    (length '(2 4 6)))
