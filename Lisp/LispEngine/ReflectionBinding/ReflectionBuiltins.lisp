; The reader reads ".Equals" as "(dot Equals)".
; Here, we expand "dot" as a macro.
; So we end up with:
; (.Equals "one" "two") =>
; ((dot Equals) "one" "two") =>
; ((curry invoke-instance "Equals") "one" "two")
(define-macro dot (method)
    `(,curry ,invoke-instance ,(symbol->string method)))
