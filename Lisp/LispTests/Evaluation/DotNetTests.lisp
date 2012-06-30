(setup
    ; Bring in all static methods in the mscorlib assembly
    (ref mscorlib))
(tests
    ; Call the primitive invoke-method function
    (invoke-instance-simple
        "42"
        (invoke-instance "ToString" 42))
    (invoke-instance-with-args1
        #t
        (invoke-instance "Equals" "hello world" "hello world"))
    (invoke-instance-with-args2
        #f
        (invoke-instance "Equals" "hello" "nothello"))

    (invoke-instance-macro
        "42"
        (.ToString 42))
    ; Call a static method returning a value type (Boolean)
    (static-method
        #t
        (System.Convert.ToBoolean "true"))
    ; Call a static method returning void
    (static-method-void
        ()
        (System.Console.WriteLine "hello"))
    ; Call an instance method (Equals) on a reference type (String)
    (instance-method-reftype
        #t
        (.Equals "hello" "hello"))
    ; Call an instance method (Equals) on a value type (Int32), passing a value type (Int32)
    (instance-method-valuetype
        #t
        (.Equals 1 1))
    ; Call an instance method (Equals) on a value type (Int32), passing a reference type (String)
    (instance-method-valuetype-boxed-arg
        #f
        (.Equals 1 "hello"))

    ; Evaluation always has to be via the main stack rather than
    ; creating nested "Evaluator" instances.
    ; Otherwise, as evidenced below, the "exception" (one example
    ; of how we can use continuations), is only "thrown" to the
    ; .Net method call site rather than the original source
    ; of the continuation.
    (args-obey-continuations
        "failed"
        (let/cc error
            (.Equals "failed" (error "failed"))))

    (static-method-continuations
        "failed"
        (let/cc error
            (System.Console.WriteLine (error "failed"))))

    (static-method-is-function
        (#t #f)
        (map System.Convert.ToBoolean '("true" "false")))

    (instance-method-as-function
        ("23" "34")
        (map (curry invoke-instance "ToString") '(23 34)))
)
