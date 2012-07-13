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

(define (make-repl prompt repl-env)
    (define prompt (curry System.Console.Write prompt))
    (define env-with-exit (extend repl-env 'exit nil))
    (define (repl)
        (let/cc return
            (define exit (curry return nil))
            ; Change the "exit" function of the
            ; repl environment to break out of this
            ; loop
            ; We have to do it this way so that defines
            ; made in the repl loop are persisted
            (eval `(set! exit ,exit) env-with-exit)
            ; Read an expression, but exit the loop
            ; if it's eof.
            (define (check-read)
                (let next (read console)
                     (if (eof-object? next)
                         (exit)
                         next)))
            (try
                (prompt)
                (with (expr (check-read)
                       result (eval expr env-with-exit))
                      (display result))
             catch (msg c)
                (display-error msg c)
                (let debug-repl (make-repl "debug> " (get-env c))
                    (debug-repl))
            )
            (repl))
    repl))

(let repl (make-repl "FCLisp> " (env))
    (repl))
