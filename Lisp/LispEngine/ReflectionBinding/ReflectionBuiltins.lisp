; The reader reads ".Equals" as "(dot Equals)".
; Here, we expand "dot" as a macro.
; So we end up with:
; (.Equals "one" "two") =>
; ((dot Equals) "one" "two") =>
; ((make-instance-method "Equals") "one" "two")
(define-macro dot (method)
    `(,make-instance-method ,(symbol->string method)))
