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

; Create a new environment with the specified definitions
(define (extend e . definitions)
    (define (extend1 (symbol definition) e)
        (eval
            `(,begin
                (,define ,symbol ,definition)
                (,env))
            e))
    (fold-right extend1 e definitions))

(define (get-debug-env (msg c))
    (extend (get-env c)
            `(trace ,(curry display-error msg c))
            `(resume ,c)))

(define (repl prompt repl-env)
    (define prompt (curry System.Console.Write prompt))
    (define last-error nil)
    (define (debug)
        (if (nil? last-error)
            "No error occurred"
            (repl "debug> " (get-debug-env last-error))))

    (let/cc return
        (define exit (curry return nil))
        (define env-with-exit-and-debug
            (extend repl-env `(exit ,exit) `(debug ,debug)))

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
                       result (eval expr env-with-exit-and-debug))
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
