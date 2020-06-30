

#https://refactoring.guru/ru/design-patterns/singleton/python/example
class MetaSingl(type):
    """
    В Python класс Одиночка можно реализовать по-разному. Возможные способы
    включают себя базовый класс, декоратор, метакласс. Мы воспользуемся
    метаклассом, поскольку он лучше всего подходит для этой цели.
    """

    _instances = {}

    def __call__(cls, *args, **kwargs):
        """
        Данная реализация не учитывает возможное изменение передаваемых
        аргументов в `__init__`.
        """
        if cls not in cls._instances:
            instance = super().__call__(*args, **kwargs)
            cls._instances[cls] = instance
        return cls._instances[cls]


class Singleton(metaclass=MetaSingl):
    __anyProp = "any text"

    def some_business_logic(self):
        print(self)

    @property
    def anyProp(self):
        return self.__anyProp
 
    @anyProp.setter
    def anyProp(self, anyProp):
        self.__anyProp = anyProp