(ref mscorlib)
(define console (open-input-stream (System.Console.get_In)))
(define display (curry System.Console.WriteLine "-> {0}"))

(define (display-error msg c)
    (define writeerr (curry .WriteLine (System.Console.get_Error)))
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
    (let/cc return
        (define exit (curry return nil))
        (define env-with-exit (extend repl-env 'exit exit))
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
                       result (eval expr env-with-exit))
                      (display result))
             catch (msg c)
                (display-error msg c)
                (repl "debug> " (get-env c)))
            (loop))
        (loop)))

(repl "FCLisp> " (env))
