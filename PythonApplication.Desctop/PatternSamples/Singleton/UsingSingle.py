from SimpleSingl import *
from LazySingl import *
from MetaSingl import *


def testSimpleSingle():
    print("Simple singletone test")
    ss = SimpleSingl()
    print("Object created", ss)
    ss1 = SimpleSingl()
    print("Object created", ss1)

    ss.age = 20
    print(ss1.age)

    

def testLazySingle():
    print("LAzy singletone test")
    ls = LazySingl() ## class initialized, but object not created
    print("Object created", LazySingl.getInstance()) # Object gets created here
    ls1 = LazySingl() ## instance already created

    if id(ls) == id(ls1):
        print("Singleton works, both variables contain the same instance.")
    else:
        print("Singleton failed, variables contain different instances.")

    ls.age = 10
    print(ls1.age)


def testMetaSingle():
    print("Meta singletone test")

    s1 = Singleton()
    s2 = Singleton()

    if id(s1) == id(s2):
        print("Singleton works, both variables contain the same instance.")
    else:
        print("Singleton failed, variables contain different instances.")

    s1.anyProp = "New singletone text"
    print(s2.anyProp)


if __name__ == "__main__":
    testSimpleSingle()
    testLazySingle()
    testMetaSingle()
