# US 5.1.21

## 1. Context

This task appears in the start of the project's development, to be able to edit a existing operation type.


## 2. Requirements

**US 5.1.21** As an Admin, I want to edit existing operation types, so that I can update or correct information about the procedure. 

**Acceptance Criteria:**

- Admins can search for and select an existing operation type to edit. 
- Editable fields include operation name, required staff by specialization, and estimated duration. 
- Changes are reflected in the system immediately for future operation requests. 
- Historical data is maintained, but new operation requests will use the updated operation type information.


**Dependencies/References:**

It is also required that the user is registered and logged in as an admin.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:


```
Q: In a previous answer you stated that "The type of operation is associated with a given specialty".

In another answer you said "a team of 1 doctor with specialization X and one nurse with specialization Y" (regarding the required staff for a said type of operation).

From the specifications document and the additional document with the 10 most common types of operations, we have two specializations: orthopedics and cardiology.

My question is: If the operation type already has a specialization associated, how can we have staff with different specializations?

 

What do you understand by specialization? Is it cardiology/orthopedics? Or anaesthesist/circulating/...

A: the operation is mainly associated with one specialization, but for a specific operation it might require a team with multiple specializations.

cardiology, orthopedics, anaesthesist are specializations that either doctors or nurses can have.

the circulating technician is a different type of medical professional. for now the system doesn't need to support technicians
```

```
Q: Regarding the operation type editions, when does an edition affects an operation request or appoiment?

Let's say the request status is "requested", is the edition applicable to that request? What if the request status is "planned"?

The same for appoitment. I'd assume that operation type editions do not affect completed or cancelled appoitments, but what about the ones with the "scheduled" status?

Can you please clarify this aspect of the system please?

A: surgeries will "point" to the operation type version that is/was in force at the time of scheduling
```

```
Q: Can you clarify? 
"Historical data is maintained, but new operation requests will use the updated operation type information. "

A: 
it means that if an operation type is changed we need to keep track of its changes. For instance,
Operation Type "A" is defined as taking 30 minutes preparation, 1h surgery and 30 minutes cleaning with a team of 1 doctor with specialization X and one nurse with specialization Y
some operations are requested, scheduled and performed based on this definition
after sometime, the hospital changes its procedures and defines the operation type "A" as needing 30 min prep, 30 min. surgery and 30 min. cleaning, with a team of 3 doctors and one nurse.
new operations will be requested, scheduled and performed using this new definition,
however, we need to keep historical data, so that if the admin wants to know the details of an operation in the past, the system must show the operation type as it was defined at the time of the operation request
```

```
Q: 

A: 
```

The following **HTTP requests** will be implemented:
- PATCH (to edit a existing operation type)
- PUT (to edit a existing operation type)


## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Sequence Diagram (Level 1)

![SSD_Lvl1.png](SD1.svg) 


### 4.2. Sequence Diagram (Level 2)

![SSD_Lvl1.png](SD2.svg) 


### 4.3. Sequence Diagram (Level 3)

### 4.3.1 Patch (Level 3)

![SSD_Lvl1.png](SD3Patch.svg) 

### 4.3.2 Put (Level 3)

![SSD_Lvl1.png](SD3Put.svg) 

### 4.4. Applied Design Patterns

- **Domain Driven Development (DDD):** the focus is the business logic and not the implementation.
- **Data Transfer Object (DTO):** gives an abstraction layer to the domain, so that it's only presented specific information regarding the object.
- **Model View Controller (MVC):** allows the re-usability of components and promotes a more modular approach to the code, making it easier to manage and maintain.
- **Repository pattern:** allows access to data without sharing the details of data storing, like the database connection.
- **Service pattern:** helps keeping high cohesion and low coupling in the code by separating complex business logic from the rest of the system. They also promote reuse, as multiple parts of the system can use the same service to perform common operations.
- **Test Driven Development (TDD):** planning the tests previously to the code gives orientation lines to the development process.
- **Onion Architecture:** concentric layers structure that puts the Domin Model as the core. Promotes modularity, flexibility and testability.
- **Inversion of Control:** the responsability of object creation and dependency management belongs to a framework or external entity, so that the class doesn't need to. Promotes flexibility and decoupling.
- **Dependency Injection:** used to implement inversion of control. The dependencies are injected into a class from the outside.


### 4.5. Tests

#### 4.5.1. Unit and Integration Tests

- Unit tests for Operation Type data update 

- Unit tests for the controller focusing on the update of the operation type

- Unit tests for the service focusing on the update of the operation type


#### 4.5.2. Postman Tests

- Status code test
- Response Body Contains fields
- Response Body data validation
- Status Code for Invalid Input and errors
- Check if Resource Was updated
- Check if Data is Persisted

## 5. Implementation

The implementation of this US is according to the design, as can be seen in the SD and CD presented before.

All commits referred the corresponding issue in GitHub, using the #25 tag, as well as a relevant commit message.

It was implemented in the branch feature/21-backend-admin-edit-existing-operation-types.


## 6. Integration/Demonstration

To update a new Operation Type, run the Backoffice app and send a PUT or PATCH HTTP request with the new operation type data.

## 7. Observations

This work was guided by the project provided in ARQSI classes.