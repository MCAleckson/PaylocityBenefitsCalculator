Employee/Dependent Database and Paycheck Calculator
Mike Aleckson - 7-7-24

Notes:

1 - Employee and dependent data that would, in production, sit in a SQL database is here stored in the EmployeesController, and referenced as well from the DependentsController.  Thus, there's much iterating over lists in this example that would never be done in a system with proper CRUD functions.
2 - I did not find a way to run the integration tests while the API server process was also running.  Thus, I have to run Visual Studio twice, once with the API server process running and the other running the test fixtures.
3 - I addressed all //task requests.
4 - A few additional comments related to design decisions are found throughout the code.
