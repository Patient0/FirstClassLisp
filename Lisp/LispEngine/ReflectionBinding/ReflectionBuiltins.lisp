; The reader reads ".Equals" as "(dot () Equals)".
; Here, we expand "dot" as a macro.
; So we end up with:
; (.Equals "one" "two") =>
; ((dot '() Equals) "one" "two") =>
; ((make-instance-method "Equals") "one" "two")

; System.Console =>
; (dot System Console) =>
; (get-type "System" "Console")
(define-macro dot args
    (match args
           (() method)
                `(,make-instance-method ,(symbol->string method))
           name-parts
                `(,get-type ,@(mapcar symbol->string name-parts))))

; System.Console/WriteLine =>
; (slash (dot System Console) WriteLine) =>
; (get-static-method (get-type "System" "Console") "WriteLine")
(define-macro slash (type method)
           `(,get-static-method ,type ,(symbol->string method)))
