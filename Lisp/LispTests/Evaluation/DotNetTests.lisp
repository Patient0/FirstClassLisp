; Call a static method
; TODO: invent a syntax for this (not sure the '.'s parse well)
; (static-method
;     #t
;     (System.Convert.ToBoolean "true"))
; Call an instance method (Equals) on a reference type (String)
(instance-method-reftype
    #t
    (.Equals 'hello 'hello))
; Call an instance method (Equals) on a value type (Int32), passing a value type (Int32)
(instance-method-valuetype
    #t
    (.Equals 1 1))
; Call an instance method (Equals) on a value type (Int32), passing a reference type (String)
(instance-method-valuetype-boxed-arg
    #f
    (.Equals 1 'hello))
