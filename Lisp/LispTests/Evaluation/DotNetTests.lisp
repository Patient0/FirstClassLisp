; Call a static method returning a value type (Boolean)
(static-method
    #t
    (System.Convert.ToBoolean 'true))
; Call a static methos returning void
(static-method-void
	()
	(System.Console.WriteLine 'hello))
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
