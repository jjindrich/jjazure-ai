from litellm import completion
import os

## set ENV variables
os.environ["AZURE_API_KEY"] = "4a9ecd0d36744b7999f0ffbb2e0ffdd7"
os.environ["AZURE_API_BASE"] = "https://jjazscoai.openai.azure.com/"
os.environ["AZURE_API_VERSION"] = "2023-05-15"

messages = [{ "content": "Hello, how are you?","role": "user"}]

# openai call
response = completion(model="azure/jjconsole", messages=messages)

message = response.choices[0].message.content

print(message)