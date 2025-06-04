from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
import pandas as pd
import uuid
from sklearn.model_selection import train_test_split
from sklearn.neighbors import KNeighborsClassifier
from sklearn.naive_bayes import GaussianNB
from sklearn.linear_model import LogisticRegression
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import f1_score, precision_score, recall_score, accuracy_score

app = FastAPI()

class KNNDatasetInput(BaseModel):
    data: List[List[str]]
    analysis_columns: List[str]
    target_column: str
    training_size: float
    neighbors: int
    distance_metric: str

class BayesDatasetInput(BaseModel):
    data: List[List[str]]
    analysis_columns: List[str]
    target_column: str
    training_size: float

class LogisticRegressionInput(BaseModel):
    data: List[List[str]]
    analysis_columns: List[str]
    target_column: str
    training_size: float

class ModelOutput(BaseModel):
    f1: float
    precision: float
    recall: float
    accuracy: float
    request_id: str


def prepare_dataset(training_size, data, analysis_columns, target_column):
    
    full_columns = analysis_columns + [target_column]
    
    df = pd.DataFrame(data, columns=full_columns)

    
    for col in df.columns:
        if col != target_column:
            df[col] = pd.to_numeric(df[col], errors='coerce')

    df = df.dropna()
    X = df[analysis_columns]
    y = df[target_column]


    le = LabelEncoder()
    y = le.fit_transform(y)

    test_size = 1 - (training_size / 100)
    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=test_size, random_state=42)

    return X_train, X_test, y_train, y_test


@app.get("/health")
def health_check():
    return {"status": "ok"}
            
@app.post("/knn", response_model=ModelOutput)
async def run_knn(input_data: KNNDatasetInput):
    try:
        request_id = str(uuid.uuid4())

        X_train, X_test, y_train, y_test = prepare_dataset(input_data.training_size, input_data.data, input_data.analysis_columns, input_data.target_column)

        if input_data.neighbors > len(X_train):
            raise HTTPException(status_code=400, detail = f"The number of neighbors ({input_data.neighbors}) cannot be greater than the number of rows in the training dataset ({len(X_train)}).")
        distance_metric = input_data.distance_metric.strip().lower()
        knn = KNeighborsClassifier(n_neighbors=input_data.neighbors, metric=distance_metric)
        knn.fit(X_train, y_train)
        y_pred = knn.predict(X_test)

        metrics = {
            "f1": f1_score(y_test, y_pred, average='weighted'),
            "precision": precision_score(y_test, y_pred, average='weighted', zero_division=0),
            "recall": recall_score(y_test, y_pred, average='weighted', zero_division=0),
            "accuracy": accuracy_score(y_test, y_pred),
            "request_id": request_id
        }

        print("KNN Response:", metrics)

        return ModelOutput(**metrics)

    except Exception as e:
        print("Error in KNN:", str(e))
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/bayes", response_model=ModelOutput)
async def run_bayes(input_data: BayesDatasetInput):
    try:
        request_id = str(uuid.uuid4())

        X_train, X_test, y_train, y_test = prepare_dataset(input_data.training_size, input_data.data, input_data.analysis_columns, input_data.target_column)

        nb = GaussianNB()
        nb.fit(X_train, y_train)
        y_pred = nb.predict(X_test)

        metrics = {
            "f1": f1_score(y_test, y_pred, average='weighted'),
            "precision": precision_score(y_test, y_pred, average='weighted', zero_division=0),
            "recall": recall_score(y_test, y_pred, average='weighted', zero_division=0),
            "accuracy": accuracy_score(y_test, y_pred),
            "request_id": request_id
        }

        print("Bayes Response:", metrics)

        return ModelOutput(**metrics)

    except Exception as e:
        print("Error in Bayes:", str(e))
        raise HTTPException(status_code=500, detail=str(e))
    
@app.post("/lr", response_model=ModelOutput)
async def run_lr(input_data: LogisticRegressionInput):
    try:
        request_id = str(uuid.uuid4())

        X_train, X_test, y_train, y_test = prepare_dataset(input_data.training_size, input_data.data, input_data.analysis_columns, input_data.target_column)

        lr = LogisticRegression()
        lr.fit(X_train, y_train)
        y_pred = lr.predict(X_test)

        metrics = {
            "f1": f1_score(y_test, y_pred, average='weighted'),
            "precision": precision_score(y_test, y_pred, average='weighted', zero_division=0),
            "recall": recall_score(y_test, y_pred, average='weighted', zero_division=0),
            "accuracy": accuracy_score(y_test, y_pred),
            "request_id": request_id
        }

        print("LR Response:", metrics)

        return ModelOutput(**metrics)

    except Exception as e:
        print("Error in LR:", str(e))
        raise HTTPException(status_code=500, detail=str(e))