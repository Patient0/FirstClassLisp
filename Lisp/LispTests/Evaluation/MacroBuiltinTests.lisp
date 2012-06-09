; This .lisp test file is for those 'builtins' which
; have been defined in terms of macros
; define-macro is itself a macro
(define-macro 8
    (begin
        ; Just duplicate the original 'let' macro
        (define-macro slet (var value body)
                `((,lambda (,var) ,body) ,value))
        (slet x 5 (+ x 3))))
; Our own homegrown 'list comprehension' syntax
; which is defined simple as a macro that expands to a 'map' call
(loop (1 4 9 16)
    (loop x '(1 2 3 4)
        (* x x)))
