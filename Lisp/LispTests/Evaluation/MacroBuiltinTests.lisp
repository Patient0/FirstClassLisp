; This .lisp test file is for those 'builtins' which
; have been defined in terms of macros
; define-macro is itself a macro
(define-macro 8
    (begin
        ; Just duplicate the original 'let' macro
        (define-macro slet (var value body)
                `((,lambda (,var) ,body) ,value))
        (slet x 5 (+ x 3))))
; We ought to support the traditional scheme
; "define function" syntax
(define-function 25
    (begin
        (define (square x) (* x x))
        (square 5)))

(define-function-two-args 18
    (begin
        (define (subtract x y) (- x y))
        (subtract 21 3)))

(define-function-multiple-sub-expressions 10
    (begin
        (define (subtract x y)
            35 ; this is evaluated by ignored
            (- x y))
        (subtract 15 5)))

; Our own homegrown 'list comprehension' syntax
; which is implemented in terms of map
(loop (1 4 9 16)
    (loop x '(1 2 3 4)
        (* x x)))

; 'with' is like let but allows multiple variable
; definitions.
(with (5 100 7)
    (with (x 5 y 100 z 7)
        (list x y z)))
