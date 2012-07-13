(ref mscorlib)
(define console (open-input-stream (System.Console.get_In)))
(define display (curry System.Console.WriteLine "-> {0}"))
(define writeerr (curry .WriteLine (System.Console.get_Error)))

(define (display-error msg c)
    (define (indent-list continuation-fn)
        (loop f (continuation-fn c)
            (writeerr "\t{0}" f)))
    (writeerr "ERROR: {0}" msg)
    (writeerr "Tasks:")
    (indent-list task-descriptions)
    (writeerr "Results:")
    (indent-list pending-results)
    nil)

; Make an environment in which the given symbol has the specified
; definition
(define (extend e symbol definition)
    (eval
        `(,begin
            (,define ,symbol ,definition)
            (,env))
        e))

(define (repl prompt repl-env)
    (define prompt (curry System.Console.Write prompt))
    (define last-error nil)
    (define (debug)
        (display last-error)
        (match last-error
                () "No error occurred"
                (msg c) (repl "debug> " (get-env c))))
    (let/cc return
        (define exit (curry return nil))
        (define env-with-exit (extend repl-env 'exit exit))
        (define env-with-debug (extend env-with-exit 'debug debug))

        ; Read an expression, but exit the loop
        ; if it's eof.
        (define (check-read)
            (let next (read console)
                 (if (eof-object? next)
                     (exit)
                     next)))
        (define (loop)
            (try
                (prompt)
                (with (expr (check-read)
                       result (eval expr env-with-debug))
                      (display result))
                ; Clear last error
                (set! last-error nil)
             catch error
                (set! last-error error)
                (writeerr "ERROR: {0}" (car error))
                (writeerr "(debug) to enter debug repl"))
            (loop))
        (loop)))

(repl "FCLisp> " (env))
