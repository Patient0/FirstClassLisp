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
)
