; Functions for input and output
; Defined using .Net bindings for the most part

; Read a file into a list of s-expressions
; Not very elegant until we build in some sort of RAII
(define (read-file filename)
    (let-cc return 
        (let file-stream (System.IO.File/OpenRead filename)
            (try
                (with* (text-reader (new System.IO.StreamReader file-stream)
                        input (open-input-stream text-reader))
                    (define (loop so-far)
                        (let next (read input)
                            (if (eof-object? next)
                                (begin
                                    (.Dispose file-stream)
                                    (return (reverse so-far)))
                            (loop (cons next so-far)))))
                    (loop nil))
             catch (msg c)
                ; Not quite ideal. Ideally we'd "throw" the
                ; original continuation along with this.
                (.Dispose file-stream)
                throw msg))))


(define pwd System.IO.Directory/GetCurrentDirectory)

; Load an execute a lisp file using the specified
; environment
(define (run filename run-environment)
    (define last-result nil)
    (loop expr (read-file filename)
        (set! last-result (eval expr run-environment)))
    last-result)

(define system-console System.Console)
(define console (open-input-stream (system-console/get_In)))
(define display (curry system-console/WriteLine "-> {0}"))
(define writeerr (curry .WriteLine (system-console/get_Error)))
(define write system-console/Write)
(define write-line system-console/WriteLine)

