ReQuiz
======

Introduction
------------

This is my COMP4 coursework that I submitted to the AQA as a part of my Computing
GCE A-Level qualification.

ReQuiz is a client-server system that tests students on their knowledge of regular
expression syntax with randomly-generated questions.

Compilation
-----------

ReQuiz is written in C# using Microsoft Visual C# 2010 Express. Opening the 
*ReQuiz.sln* in VS and building the solution should do the trick.
Only Windows is supported so far, maybe I'll migrate it to Mono sometime.

Usage
-----

As a part of the coursework, I also had to submit a report on the project with
various object diagrams, low- and high-level designs, etc. However, there is some
personal data in the report, as well as many things that are not really needed
(such as the testing evidence or the listing of the source code), so I will upload it
when I've dealt with that.

The instructions for using the program should be fairly obvious from the floating hints
and button captions. The common usage pattern is as follows:

* The teacher enters the quiz parameters into the server and starts it.
* The students connect to the server using their clients.
* The students answer the fetched questions and request hints to be shown.
* When a student has finished the test, he/she submits all answers to the server.
* The server marks the answers and returns the mark to the student.
* The progress is displayed in a log window on the server for the teacher to see.
* In the end of the test, the teacher shuts down the server.
* The teacher can review the marks attained and export them into a CSV file.

There are two question types that can be generated:

1. Given a regular expression, write a string that matches it.
2. Given a regular expression and 4 strings, choose the one that matches it.

Hints are also supported, they decrease the overall score by 1 mark (each question is worth 2):

1. A matching string is generated and some letters removed from it.
2. Two wrong strings are removed.

There are 3 files relevant to ReQuiz:

###ReQuizServer.exe
The server, it allows the teacher to set up the quiz parameters, listens for 
incoming connections and sends and marks the questions. The teacher can also 
export the marks into a CSV file. The server prevents students from taking a
quiz twice by tracking their Windows usernames that are reported by the client 
(not terribly secure, I know).

###ReQuizClient.exe
The client, installed on every student's computer. It fetches the questions and
provides the student with an interface to answer them.

###ReQuizCommon.dll
The regular expression engine that both the server and the client use.

