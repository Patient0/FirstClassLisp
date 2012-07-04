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

    (nested-error
        "ERROR"
        (try
            (* 6 undefined)
        catch x "ERROR"))

    ; This test demonstrates why we cannot just
    ; have a "setErrorHandler" in the continuation.
    ;
    ; We need a stack of error handlers so that
    ; things behave as we expect if we perform an
    ; eval of an eval.
    (nested-try-catch
        "ERROR1"
        (try
            (try
                'undefined
            catch "ERROR1")
            'undefined
        catch "ERROR2"))
)
