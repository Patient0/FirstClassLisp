(tests
    ; "(env)" is an f-expression that returns the current environment.
    ; This environment is currently "opaque".
    ; "eval" is then a regular function that takes this environment
    ; and evaluates the expression.
    (simple-eval
        15
        (eval '(* 3 5) (env)))
    (simple-read
        (* 5 3)
        (read (open-input-string "(* 5 3)")))

    (eof-detect
        (#t #f)
        (map (compose eof-object? read open-input-string)
                 '("" "(* 5 3)")))

    ; Allow passing in an extra parameter which is a function that
    ; will receive an error notification.
    (eval-with-error
        "ERROR"
        (eval 'an-undefined-symbol (env) (lambda (x) "ERROR")))

    (eval-nested-with-error
        "ERROR"
        (eval '(* 6 undefined) (env) (lambda (x) "ERROR")))

    ; This test demonstrates why we cannot just
    ; have a "setErrorHandler" in the continuation.
    ;
    ; We need a stack of error handlers so that
    ; things behave as we expect if we perform an
    ; eval of an eval.
    (eval-error-handler-not-captured
        "ERROR1"
        (begin
            (define error-expr 'undefined)
            (define (error-handler-1 msg) "ERROR1")
            (define (error-handler-2 msg) "ERROR2")
            (define eval-error-expr
                '(begin
                    ; This part sets up error handler 2.
                    ; But whether an error occured or not,
                    ; error-handler-2 should only be relevant
                    ; for this nested eval only.
                    (eval 5 (env) error-handler-2)
                    undefined))
                    
            (eval eval-error-expr (env) error-handler-1)))
)
